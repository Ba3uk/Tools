using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HardTutor
{
    [AddComponentMenu("Event/Hard Tutor Raycaster")]
    [RequireComponent(typeof(Canvas))]
    public sealed class HardTutorRaycaster : BaseRaycaster
    {
        private enum BlockingObjectsType
        {
            None = 0,
            TwoD = 1,
            ThreeD = 2,
            All = 3,
        }

        [SerializeField] private bool _ignoreReversedGraphics = true;
        [SerializeField] private BlockingObjectsType _blockingObjects = BlockingObjectsType.None;
        private const int NO_EVENT_MASK_SET = -1;
        [SerializeField] private LayerMask _blockingMask = NO_EVENT_MASK_SET;

        private Canvas _canvas;

        private Canvas Canvas
        {
            get
            {
                if (_canvas != null)
                    return _canvas;

                _canvas = GetComponent<Canvas>();
                return _canvas;
            }
        }

        private Camera _mainCamera;

        private Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }

        private static string _lastLoggedPath;
        private readonly List<Graphic> _raycastResultsBuffer = new List<Graphic>();
        private static readonly List<Graphic> SortedGraphics = new List<Graphic>();
        private static bool _isHardTutorActivated;
        private static IReadOnlyList<int> _hardTutorInstanceIds;

        public override Camera eventCamera
        {
            get
            {
                var renderMode = Canvas.renderMode;
                if (renderMode == RenderMode.ScreenSpaceOverlay ||
                    (renderMode == RenderMode.ScreenSpaceCamera && Canvas.worldCamera == null))
                    return null;

                var canvasCamera = Canvas.worldCamera;

                return canvasCamera ? canvasCamera : MainCamera;
            }
        }

        public override int renderOrderPriority => Canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? Canvas.rootCanvas.renderOrder
            : base.renderOrderPriority;

        public override int sortOrderPriority => Canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? Canvas.sortingOrder
            : base.sortOrderPriority;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (Canvas == null)
            {
                return;
            }

            var canvasGraphics = GraphicRegistry.GetRaycastableGraphicsForCanvas(Canvas);
            if (canvasGraphics == null || canvasGraphics.Count == 0)
            {
                return;
            }

            var currentEventCamera = eventCamera;
            var displayIndex = GetDisplayIndex(currentEventCamera);
            if (!TryGetEventPosition(eventData, displayIndex, out var eventPosition))
            {
                return;
            }

            if (IsEventOutsideOfViewport(eventPosition, currentEventCamera, displayIndex))
            {
                return;
            }

            var ray = CreateRay(currentEventCamera, eventPosition);
            var hitDistance = FindHitDistance(currentEventCamera, ray);

            _raycastResultsBuffer.Clear();
            Raycast(currentEventCamera, eventPosition, canvasGraphics, _raycastResultsBuffer);

            var totalCount = _raycastResultsBuffer.Count;
            var previousCount = resultAppendList.Count;
            for (var index = 0; index < totalCount; index++)
            {
                var resultGameObject = _raycastResultsBuffer[index].gameObject;

                if (!NeedAppendGraphic(currentEventCamera, resultGameObject))
                {
                    continue;
                }

                var resultTransform = resultGameObject.transform;
                var resultTransformForward = resultTransform.forward;

                if (!TryFindDistance(currentEventCamera, resultTransformForward, resultTransform, ray, hitDistance,
                    out var distance))
                {
                    continue;
                }

                var castResult = new RaycastResult
                {
                    gameObject = resultGameObject,
                    module = this,
                    distance = distance,
                    screenPosition = eventPosition,
                    displayIndex = displayIndex,
                    index = resultAppendList.Count,
                    depth = _raycastResultsBuffer[index].depth,
                    sortingLayer = Canvas.sortingLayerID,
                    sortingOrder = Canvas.sortingOrder,
                    worldPosition = ray.origin + ray.direction * distance,
                    worldNormal = -resultTransformForward
                };

                resultAppendList.Add(castResult);
            }

            if (resultAppendList.Count > previousCount)
            {
                LogFullScenePath(resultAppendList[previousCount].gameObject);
            }
        }

        public static void StartHardTutor(IReadOnlyList<int> hardTutorObjectsId)
        {
            _isHardTutorActivated = true;
            _hardTutorInstanceIds = hardTutorObjectsId;
        }

        public static void StopHardTutor()
        {
            _isHardTutorActivated = false;
            _hardTutorInstanceIds = null;
        }

        private bool TryFindDistance(
            Camera currentCamera,
            Vector3 resultTransformForward,
            Transform resultTransform,
            Ray ray,
            float hitDistance,
            out float distance)
        {
            distance = 0;
            if (currentCamera == null || Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                distance = 0;
            }
            else
            {
                // http://geomalgorithms.com/a06-_intersect-2.html
                distance = (Vector3.Dot(resultTransformForward, resultTransform.position - ray.origin) /
                            Vector3.Dot(resultTransformForward, ray.direction));

                // Check to see if the go is behind the camera.
                if (distance < 0)
                {
                    return false;
                }
            }

            return distance < hitDistance;
        }

        private bool NeedAppendGraphic(Camera currentCamera, GameObject target)
        {
            if (!_ignoreReversedGraphics)
            {
                return true;
            }

            bool appendGraphic;
            if (currentCamera == null)
            {
                // If we dont have a camera we know that we should always be facing forward
                var dir = target.transform.rotation * Vector3.forward;
                appendGraphic = Vector3.Dot(Vector3.forward, dir) > 0;
            }
            else
            {
                // If we have a camera compare the direction against the cameras forward.
                var cameraTransform = currentCamera.transform;
                var cameraForward = cameraTransform.rotation * Vector3.forward * currentCamera.nearClipPlane;
                var deltaForward = target.transform.position - cameraTransform.position - cameraForward;
                appendGraphic = Vector3.Dot(deltaForward, target.transform.forward) >= 0;
            }

            return appendGraphic;
        }

        private float FindHitDistance(Camera currentEventCamera, Ray ray)
        {
            var hitDistance = float.MaxValue;
            if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay || _blockingObjects == BlockingObjectsType.None)
            {
                return hitDistance;
            }

            var distanceToClipPlane = 100.0f;

            if (currentEventCamera != null)
            {
                var projectionDirection = ray.direction.z;
                distanceToClipPlane = Mathf.Approximately(0.0f, projectionDirection)
                    ? Mathf.Infinity
                    : Mathf.Abs((currentEventCamera.farClipPlane - currentEventCamera.nearClipPlane) /
                                projectionDirection);
            }

#if PACKAGE_PHYSICS
            if (_blockingObjects == BlockingObjectsType.ThreeD || _blockingObjects == BlockingObjectsType.All)
            {
                if (ReflectionMethodsCache.Singleton.raycast3D != null)
                {
                    var hits =
                        ReflectionMethodsCache.Singleton.raycast3DAll(ray, distanceToClipPlane, (int)_blockingMask);
                    if (hits.Length > 0)
                    {
                        hitDistance = hits[0].distance;
                    }
                }
            }
#endif

#if PACKAGE_PHYSICS2D
            if (_blockingObjects == BlockingObjectsType.TwoD || _blockingObjects == BlockingObjectsType.All)
            {
                if (ReflectionMethodsCache.Singleton.raycast2D != null)
                {
                    var hits =
                        ReflectionMethodsCache.Singleton.getRayIntersectionAll(ray, distanceToClipPlane, (int)_blockingMask);
                    if (hits.Length > 0)
                    {
                        hitDistance = hits[0].distance;
                    }
                }
            }
#endif

            return hitDistance;
        }

        private int GetDisplayIndex(Camera currentEventCamera)
        {
            int displayIndex;
            if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay || currentEventCamera == null)
                displayIndex = Canvas.targetDisplay;
            else
                displayIndex = currentEventCamera.targetDisplay;
            return displayIndex;
        }

        private static bool IsEventOutsideOfViewport(Vector3 eventPosition, Camera currentEventCamera, int displayIndex)
        {
            Vector2 pos;
            if (currentEventCamera == null)
            {
                // Multiple display support only when not the main display. For display 0 the reported
                // resolution is always the desktops resolution since its part of the display API,
                // so we use the standard none multiple display method. (case 741751)
                float w = Screen.width;
                float h = Screen.height;
                if (displayIndex > 0 && displayIndex < Display.displays.Length)
                {
                    w = Display.displays[displayIndex].systemWidth;
                    h = Display.displays[displayIndex].systemHeight;
                }

                pos = new Vector2(eventPosition.x / w, eventPosition.y / h);
            }
            else
            {
                pos = currentEventCamera.ScreenToViewportPoint(eventPosition);
            }

            return pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f;
        }

        private static bool TryGetEventPosition(PointerEventData eventData, int displayIndex, out Vector3 eventPosition)
        {
            eventPosition = Display.RelativeMouseAt(eventData.position);
            var isSystemSupportManyDisplays = eventPosition != Vector3.zero;
            if (isSystemSupportManyDisplays)
            {
                var displayIndexOfEvent = (int) eventPosition.z;
                var isEventOnAnotherDisplay = displayIndexOfEvent != displayIndex;
                if (isEventOnAnotherDisplay)
                {
                    return false;
                }
            }
            else
            {
                eventPosition = eventData.position;
            }

            return true;
        }

        private static Ray CreateRay(Camera currentEventCamera, Vector3 eventPosition)
        {
            var ray = new Ray();

            if (currentEventCamera != null)
            {
                ray = currentEventCamera.ScreenPointToRay(eventPosition);
            }

            return ray;
        }

        private static void Raycast(
            Camera eventCamera,
            Vector2 pointerPosition,
            IList<Graphic> foundGraphics,
            List<Graphic> results)
        {
            var totalCount = foundGraphics.Count;
            for (var i = 0; i < totalCount; ++i)
            {
                var graphic = foundGraphics[i];

                if (GraphicIsRaycastTarget(graphic) &&
                    PointIsInGraphicRectangle(graphic, eventCamera, pointerPosition) &&
                    GraphicAvailableForHardTutor(graphic) &&
                    GraphicIsNotTooFar(graphic, eventCamera) &&
                    RaycastHitThisGraphic(graphic, eventCamera, pointerPosition))
                {
                    SortedGraphics.Add(graphic);
                }
            }

            SortedGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
            var count = SortedGraphics.Count;
            for (var index = 0; index < count; index++)
            {
                var graphic = SortedGraphics[index];
                results.Add(graphic);
            }

            SortedGraphics.Clear();
        }

        private static bool GraphicAvailableForHardTutor(Graphic graphic)
        {
            if (!_isHardTutorActivated)
            {
                return true;
            }

            var instanceID = graphic.gameObject.GetInstanceID();
            var count = _hardTutorInstanceIds.Count;
            for (var index = 0; index < count; index++)
            {
                if (_hardTutorInstanceIds[index] == instanceID)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool GraphicIsRaycastTarget(Graphic graphic)
        {
            const int depthWhichMeansThatObjectIsNotDrawn = -1;
            var isDrawnByCanvas = graphic.depth != depthWhichMeansThatObjectIsNotDrawn;
            var isRaycastTarget = graphic.raycastTarget;
            var isGeometryNotIgnored = !graphic.canvasRenderer.cull;

            return isRaycastTarget && isGeometryNotIgnored && isDrawnByCanvas;
        }

        private static bool PointIsInGraphicRectangle(Graphic graphic, Camera eventCamera, Vector2 pointerPosition)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition,
                eventCamera, graphic.raycastPadding);
        }

        private static bool GraphicIsNotTooFar(Graphic graphic, Camera eventCamera)
        {
            return eventCamera == null ||
                   eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z <= eventCamera.farClipPlane;
        }

        private static bool RaycastHitThisGraphic(Graphic graphic, Camera eventCamera, Vector2 pointerPosition)
        {
            return graphic.Raycast(pointerPosition, eventCamera);
        }

        private void LogFullScenePath(GameObject resultGameObject)
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl))
            {
                var fullPath = GetFullPath(resultGameObject.transform);
                if (!fullPath.Equals(_lastLoggedPath))
                {
                    _lastLoggedPath = fullPath;
                    Debug.Log(fullPath, resultGameObject);
                }
            }
#endif
        }

        private static string GetFullPath(Transform trans)
        {
            if (!trans.parent)
            {
                return trans.name;
            }

            return GetFullPath(trans.parent) + "/" + trans.name;
        }
    }
}