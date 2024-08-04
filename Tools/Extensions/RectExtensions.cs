using UnityEngine;

namespace Common.Extensions
{
    public static class RectExtensions
    {
        public static Rect Extend(this Rect rect, Vector2 point)
        {
            return new Rect
            {
                min = Vector2.Min(rect.min, point),
                max = Vector2.Max(rect.max, point)
            };
        }

        public static Rect Extend(this Rect rect, Rect anotherRect)
        {
            return rect.Extend(anotherRect.min).Extend(anotherRect.max);
        }

        public static Vector2 GetCenterTop(this Rect rect)
        {
            return new Vector2(rect.center.x, rect.max.y);
        }
        
        public static Vector2 GetCenterBottom(this Rect rect)
        {
            return new Vector2(rect.center.x, rect.min.y);
        }
        
        public static Vector2 GetCenterRight(this Rect rect)
        {
            return new Vector2(rect.max.x, rect.center.y);
        }
        
        public static Vector2 GetCenterLeft(this Rect rect)
        {
            return new Vector2(rect.min.x, rect.center.y);
        }
    }
}