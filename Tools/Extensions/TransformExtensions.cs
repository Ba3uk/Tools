using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlyRewards
{
    public static class TransformExtensions
    {
        public static void SetLocalZ(this Transform target, float value)
        {
            var pos = target.localPosition;
            pos.z = value;
            target.localPosition = pos;
        }
        public static void SetLocalX(this Transform target, float value)
        {
            var pos = target.localPosition;
            pos.x = value;
            target.localPosition = pos;
        }
        public static void SetLocalY(this Transform target, float value)
        {
            var pos = target.localPosition;
            pos.y = value;
            target.localPosition = pos;
        }
        
        public static void ChangeLocalY(this Transform target, float value)
        {
            var pos = target.localPosition;
            pos.y += value;
            target.localPosition = pos;
        }
        
        public static void SetZ(this Transform target, float value)
        {
            var pos = target.position;
            pos.z = value;
            target.position = pos;
        }
        public static void SetX(this Transform target, float value)
        {
            var pos = target.position;
            pos.x = value;
            target.position = pos;
        }
        public static void SetY(this Transform target, float value)
        {
            var pos = target.position;
            pos.y = value;
            target.position = pos;
        }

        public static void SetScaleX(this Transform target, float value)
        {
            var pos = target.localScale;
            pos.x = value;
            target.localScale = pos;
        }
        
        public static void SetLayerRecursively(this Transform trans, int layer)
        {
            trans.gameObject.layer = layer;

            for (int i = 0; i < trans.childCount; i++)
            {
                SetLayerRecursively(trans.GetChild(i), layer);
            }
        }
        
        public static void SetScaleY(this Transform target, float value)
        {
            var pos = target.localScale;
            pos.y = value;
            target.localScale = pos;
        }
        public static void RemoveChilds(this Transform target, bool immediate = false)
        {
            for (int i = target.childCount - 1; i >= 0; i--)
            {
                if(!Application.isPlaying || immediate) {
                    Object.DestroyImmediate(target.GetChild(i).gameObject);
                }
                else {
                    Object.Destroy(target.GetChild(i).gameObject);                    
                }                    
            }
        }
        public static void HideChilds(this Transform target)
        {
            for (int i = target.childCount - 1; i >= 0; i--)
            {
                target.GetChild(i).gameObject.SetActive(false);
            }
        }
        public static List<GameObject> GetChilds(this Transform target)
        {
            var childs = new List<GameObject>(target.childCount);
            for (int i = target.childCount - 1; i >= 0; i--)
            {
                childs.Add(target.GetChild(i).gameObject);
            }
            return childs;
        }

        public static string GetHierarchyPath(this Transform target)
        {
            if (target != target.root)
            {
                return target.parent.GetHierarchyPath() + "/" + target.name;
            }
            return target.name;
        }

        public static List<Transform> GetAllChilds(this Transform trans, Func<Transform, bool> condition) {
            if(trans == null || trans.childCount == 0) {
                return new List<Transform>();
            }

            List<Transform> allChilds = new List<Transform>();
            for(int i = 0; i < trans.childCount; i++) {
                var child = trans.GetChild(i);
                if(condition(child)) {
                    allChilds.Add(child);
                }
                allChilds.AddRange(GetAllChilds(child, condition));
            }
            return allChilds;
        }
    }
}