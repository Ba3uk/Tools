using System;
using UnityEngine;

namespace Common.Extensions
{
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(this Component component, out T result, bool includeInactive = false)
            where T : Component
        {
            if (component == null)
                throw new NullReferenceException("Can't get component from parent of null");

            return TryGetComponentInChildren(component.gameObject, out result, includeInactive);
        }
        
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T result, bool includeInactive = false)
            where T : Component
        {
            if (gameObject == null)
                throw new NullReferenceException("Can't get component from parent of null");
            
            return (result = gameObject.GetComponentInChildren<T>(includeInactive)) != null;
        }
        
        public static bool TryGetComponentInParent<T>(this Component component, out T result)
            where T : Component
        {
            if (component == null)
                throw new NullReferenceException("Can't get component from parent of null");

            return TryGetComponentInParent(component.gameObject, out result);
        }
        
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result)
            where T : Component
        {
            if (gameObject == null)
                throw new NullReferenceException("Can't get component from parent of null");
            
            return (result = gameObject.GetComponentInParent<T>()) != null;
        }
        
        public static bool HasComponent<T>(this Component component)
            where T : Component
        {
            if (component == null)
                throw new NullReferenceException("Can't get component from parent of null");

            return HasComponent<T>(component.gameObject);
        }
        
        public static bool HasComponent<T>(this GameObject gameObject)
            where T : Component
        {
            if (gameObject == null)
                throw new NullReferenceException("Can't get component from parent of null");
            
            return gameObject.TryGetComponent<T>(out _);
        }
    }
}