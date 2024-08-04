using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Extensions
{
    public static class ViewExtensions
    {
        public static IDisposable SubscribeOnClick(this Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            return new ActionDisposable(() =>
            {
                button.onClick.RemoveListener(action);
            });
        }  
        
        public static IDisposable SubscribeOnClick(this ICollection<Button> buttons, UnityAction action)
        {
            foreach (var button in buttons)
            {
                button.onClick.AddListener(action);
            }
            
            return new ActionDisposable(() =>
            {
                foreach (var button in buttons)
                {
                    button.onClick.RemoveListener(action);
                }
            });
        }

        public static IDisposable SubscribeOnChanged(this Toggle toggle, UnityAction<bool> action, bool notifyOnSubscribe = false)
        {
            toggle.onValueChanged.AddListener(action);
            
            if (notifyOnSubscribe)
                action?.Invoke(toggle.isOn);
            
            return new ActionDisposable(() =>
            {
                toggle.onValueChanged.RemoveListener(action);
            });
        }
        
        public static IDisposable SubscribeOnChanged(this TMP_InputField input, UnityAction<string> action, bool notifyOnSubscribe = false)
        {
            input.onValueChanged.AddListener(action);
            
            if (notifyOnSubscribe)
                action?.Invoke(input.text);
            
            return new ActionDisposable(() =>
            {
                input.onValueChanged.RemoveListener(action);
            });
        }

        public static void Stretch(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMax = new Vector2(0, 0);
        }
    }
}