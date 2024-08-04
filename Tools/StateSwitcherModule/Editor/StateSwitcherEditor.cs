using System;
using StateScripts;
using UnityEditor;
using UnityEngine;

namespace common.StateSwitcherModule.Editor
{
    [CustomEditor(typeof(StateSwitcher))]
    public class StateSwitcherEditor : UnityEditor.Editor
    {
        private int selectedIndex;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            StateSwitcher switcher = (StateSwitcher) target;
            GUIContent stateName = new GUIContent("State Selector");
            selectedIndex = EditorGUILayout.Popup(stateName, selectedIndex, switcher.states.ToArray());

            try
            {
                switcher.SetState(switcher.states[selectedIndex]);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}