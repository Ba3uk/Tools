using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StateScripts
{
    public sealed class ImageSpriteStateElement : StateElement
    {
        [Serializable]
        private struct SpriteScheme
        {
            public string State;
            public Sprite Sprite;
        }

        [SerializeField] private List<SpriteScheme> _spriteSchemes = new List<SpriteScheme>();
        private Image _image;
        
        private Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                return _image;
            }
        }


        public override void SetState(string state)
        {
            if (_spriteSchemes.Any(x => x.State == state))
            {
                var scheme = _spriteSchemes.FirstOrDefault(x => x.State == state).Sprite;
                Image.sprite = scheme;
            }
        }
    }
}