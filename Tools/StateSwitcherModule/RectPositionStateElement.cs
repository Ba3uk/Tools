using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StateScripts
{
    public sealed class RectPositionStateElement : StateElement
    {
        [Serializable]
        private struct PositionScheme
        {
            public string State;
            public Vector2 Position;
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
                var position = _positionSchemes.FirstOrDefault(x => x.State == state).Position;
                RectTransform.anchoredPosition = position;
                LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            }
        }
    }
}