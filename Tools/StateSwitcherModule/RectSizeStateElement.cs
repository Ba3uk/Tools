using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StateScripts
{
    public sealed class RectSizeStateElement : StateElement
    {
        [Serializable]
        private struct SizeScheme
        {
            public string State;
            public RectSize RectSize;
        }
        
        [Serializable]
        private struct RectSize
        {
            public float Width;
            public float Height;
        }

        [SerializeField] private List<SizeScheme> _sizeSchemes = new List<SizeScheme>();
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
            if (_sizeSchemes.Any(x => x.State == state))
            {
                var scheme = _sizeSchemes.FirstOrDefault(x => x.State == state).RectSize;
                SetSize(RectTransform, new Vector2(scheme.Width, scheme.Height));
            }
        }
        
        private void SetSize(RectTransform trans, Vector2 newSize) 
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
            LayoutRebuilder.MarkLayoutForRebuild(trans);
        }
    }
}