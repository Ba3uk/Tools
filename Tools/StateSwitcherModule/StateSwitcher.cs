using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace StateScripts
{
    public class StateSwitcher : MonoBehaviour
    {
        public List<string> states = new();
        public List<StateElement> elements = new();

        public string SelectState { get; private set; }

        public void SetState(string state)
        {
            SetStateInstant(state);
        }

        public void SetStateInstant(string state)
        {
            SelectState = state;
            SetStateInternal(SelectState);
        }

        public string GetState()
        {
            return SelectState;
        }

        private void SetStateInternal(string state)
        {
            if (!states.Contains(state))
            {
                Debug.LogError($"There's no state '{state}'", gameObject);
                return;
            }

            foreach (var elem in elements)
            {
                elem.SetState(state);
            }
        }
    }
}