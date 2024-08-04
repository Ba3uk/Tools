using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FlyRewards
{
    public static class TransformAnimationExtensions
    {
        public static Sequence GetDoubleTween(this Transform target, Vector2 endPosition, Ease xEase, Ease yEase,
            float time, bool relative = true)
        {
            var sequence = DOTween.Sequence();

            var xTween = DOTween.To(() => relative ? target.localPosition.x : target.position.x, (a) =>
            {
                if (relative) target.SetLocalX(a);
                else target.SetX(a);
            }, endPosition.x, time);
            xTween.SetEase(xEase);
            sequence.Insert(0, xTween);

            var yTween = DOTween.To(() => relative ? target.localPosition.y : target.position.y, (a) =>
            {
                if (relative) target.SetLocalY(a);
                else target.SetY(a);
            }, endPosition.y, time);
            yTween.SetEase(yEase);
            sequence.Insert(0, yTween);

            return sequence;
        }

        public static Sequence GetDoubleTweenWithRotation(this Transform target, Vector2 endPosition, Ease xEase,
            Ease yEase, AnimationCurve xCurve, AnimationCurve yCurve, AnimationCurve zCurve, float time,
            bool relative = true)
        {
            var sequence = DOTween.Sequence();

            var xTween = DOTween.To(() => relative ? target.localPosition.x : target.position.x, (a) =>
            {
                if (relative) target.SetLocalX(a);
                else target.SetX(a);
            }, endPosition.x, time);
            xTween.SetEase(xEase);
            sequence.Insert(0, xTween);

            var yTween = DOTween.To(() => relative ? target.localPosition.y : target.position.y, (a) =>
            {
                if (relative) target.SetLocalY(a);
                else target.SetY(a);
            }, endPosition.y, time);
            yTween.SetEase(yEase);
            sequence.Insert(0, yTween);

            float tweener = 0;
            var rTween = DOTween.To(() => tweener, (a) =>
            {
                tweener = a;
                target.rotation = Quaternion.Euler(new Vector3(xCurve.Evaluate(tweener), yCurve.Evaluate(tweener),
                    zCurve.Evaluate(tweener)) * 360);
            }, 1, time);
            sequence.Insert(0, rTween);

            return sequence;
        }

        public static Tween GetTweenCurveRotation(this Transform target, AnimationCurve xCurve, AnimationCurve yCurve,
            AnimationCurve zCurve, float time, bool negate = true)
        {
            var sequence = DOTween.Sequence();

            float tweener = 0;
            Action rotDelegate;
            if (negate)
            {
                //target.Rotate(0, 180, 0);
                rotDelegate = () =>
                {
                    float backTweener = 1 - tweener;
                    target.rotation = Quaternion.Euler(360 * new Vector3(xCurve.Evaluate(tweener),
                        0.5f + yCurve.Evaluate(backTweener), zCurve.Evaluate(tweener)));
                };
            }
            else
            {
                rotDelegate = () =>
                {
                    target.rotation = Quaternion.Euler(360 * new Vector3(xCurve.Evaluate(tweener),
                        yCurve.Evaluate(tweener), zCurve.Evaluate(tweener)));
                };
            }

            var rTween = DOTween.To(() => tweener, (a) =>
            {
                tweener = a;
                rotDelegate();
            }, 1, time);
            sequence.Insert(0, rTween);

            return sequence;
        }

        public static Tween GetMoveTween(this Transform target, Vector3 endPosition, Ease ease, float time,
            bool relative = true, bool saveZ = true)
        {
            if (saveZ)
            {
                endPosition.z = relative ? target.localPosition.z : target.position.z;
            }

            var tween = DOTween.To(() => relative ? target.localPosition : target.position, (a) =>
            {
                if (relative) target.localPosition = a;
                else target.position = a;
            }, endPosition, time);
            tween.SetEase(ease);
            return tween;
        }

        public static Tween GetRotateTween(this Transform target, Vector3 endRotation, float time)
        {
            return target.DOLocalRotate(endRotation, time, RotateMode.Fast).SetEase(Ease.InCubic);
        }

        public static Tween GetRotateTween(this Transform target, Vector3 endRotation, float time, AnimationCurve curve)
        {
            return target.DOLocalRotate(endRotation, time, RotateMode.Fast).SetEase(curve);
        }

        public static Tween GetScaleTween(this Transform target, Vector3 scale, Ease ease, float time)
        {
            var tween = DOTween.To(() => target.localScale, (a) => { target.localScale = a; }, scale, time);
            tween.SetEase(ease);
            return tween;
        }

        public static Sequence GetShakeTween(this List<Transform> items, float amplitude, float period, float time,
            Ease ease)
        {
            var shake = DOTween.Sequence();
            float[] phase = new float[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                phase[i] = 0;
                var item = items[i];
                if (item != null && item.transform != null)
                {
                    var i1 = i;
                    var start = item.transform.localPosition.x;
                    var shakeTween = DOTween.To(() => phase[i1],
                        (a) =>
                        {
                            item.transform.SetLocalX(start + amplitude * (i1 % 2 == 0 ? Mathf.Sin(a) : Mathf.Cos(a)));
                        }, period + phase[i],
                        time);
                    shakeTween.SetEase(ease);
                    shake.Insert(0, shakeTween);
                }
                else
                {
#if DEV && false
                    Debug.LogError("GetShakeTween item == null");
#endif
                }
            }

            return shake;
        }

        public static Tween GetShakeTween(this Transform target, float time, Ease ease,
            float shakePower = 1f,
            float lengthModifier = 1f,
            Action onComplete = null)
        {
            var friction = 0;
            bool inited = false;
            Vector3 prevPosition = target.localPosition;
            return DOTween.To(() => friction,
                x =>
                {
                    if (!inited)
                    {
                        prevPosition = target.localPosition;
                        inited = true;
                    }

                    friction = x;
                    float posX = Mathf.Sin(friction) * (10 * shakePower);
                    float posY = Mathf.Cos(friction) * (4 * shakePower);

                    target.localPosition = new Vector3(prevPosition.x + posX, prevPosition.y + posY, prevPosition.z);
                },
                Mathf.RoundToInt(1000 * lengthModifier), time).SetEase(ease).OnComplete(() =>
            {
                target.localPosition = prevPosition;
                onComplete?.Invoke();
            });
        }

        public static Tween GetShakeTween(this Transform target, float time, AnimationCurve curve,
            float shakePower = 1f,
            float lengthModifier = 1f,
            Action onComplete = null)
        {
            var friction = 0;
            bool inited = false;
            Vector3 prevPosition = target.localPosition;
            return DOTween.To(() => friction,
                x =>
                {
                    if (!inited)
                    {
                        prevPosition = target.localPosition;
                        inited = true;
                    }

                    friction = x;
                    float posX = Mathf.Sin(friction) * (10 * shakePower);
                    float posY = Mathf.Cos(friction) * (4 * shakePower);

                    target.localPosition = new Vector3(prevPosition.x + posX, prevPosition.y + posY, prevPosition.z);
                },
                Mathf.RoundToInt(1000 * lengthModifier), time).SetEase(curve).OnComplete(() =>
            {
                target.localPosition = prevPosition;
                onComplete?.Invoke();
            });
        }
    }
}