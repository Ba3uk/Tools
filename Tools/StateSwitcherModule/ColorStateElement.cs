using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StateScripts
{
    public class ColorStateElement : StateElement
    {
        private Graphic _graphic;
        
        [Serializable]
        public struct ColorScheme
        {
            public string state;
            public Color color;
        }

        public List<ColorScheme> colorSchemes = new List<ColorScheme>();

        private Graphic Graphic
        {
            get
            {
                if (_graphic == null)
                {
                    _graphic = GetComponent<Graphic>();
                }

                return _graphic;
            }
        }

        public override void SetState(string state)
        {
            if (colorSchemes.Any(x => x.state == state))
            {
                Graphic.color = colorSchemes.FirstOrDefault(x => x.state == state).color;
            }
        }
    }
}