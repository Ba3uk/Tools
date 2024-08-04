using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateScripts
{
    public sealed class RectStretchPositionStateElement : StateElement
    {
        [Serializable]
        private struct PositionScheme
        {
            public string State;
            public RectPosition RectPosition;
        }
        
        [Serializable]
        private struct RectPosition
        {
            public float Left;
            public float Top;
            public float Right;
            public float Bottom;
        }
        
        [SerializeField] private List<PositionScheme> _positionSchemes = new List<PositionScheme>();
        private RectTransform _rectTransform;
        
        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }

        public override void SetState(string state)
        {
            if (_positionSchemes.Any(x => x.State == state))
            {
                var scheme = _positionSchemes.FirstOrDefault(x => x.State == state).RectPosition;
                RectTransform.offsetMin = new Vector2(scheme.Left, scheme.Bottom);
                RectTransform.offsetMax = new Vector2(-scheme.Right, -scheme.Top);
            }
        }
    }
}