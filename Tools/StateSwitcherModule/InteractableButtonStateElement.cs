using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StateScripts
{
    internal sealed class InteractableButtonStateElement : StateElement
    {
        [Serializable]
        private struct InteractableButtonScheme
        {
            public string State;
            public bool IsInteractable;
        }

        [SerializeField] private List<InteractableButtonScheme> _buttonSchemes = new List<InteractableButtonScheme>();
        private Button _button;
        
        private Button Button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                    
                }

                return _button;
            }
        }


        public override void SetState(string state)
        {
            if (_buttonSchemes.Any(x => x.State == state))
            {
                var scheme = _buttonSchemes.FirstOrDefault(x => x.State == state).IsInteractable;
                Button.interactable = scheme;
            }
        }
    }
}