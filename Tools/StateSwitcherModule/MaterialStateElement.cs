using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateScripts
{
    public class MaterialStateElement : StateElement
    {
        private MeshRenderer _meshRenderer;
        
        [Serializable]
        public struct MaterialScheme
        {
            public string state;
            public Material material;
        }

        [SerializeField] private List<MaterialScheme> _materialSchemes;

        private MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }

                return _meshRenderer;
            }
        }

        public override void SetState(string state)
        {
            if (_materialSchemes.Any(x => x.state == state))
            {
                MeshRenderer.material = _materialSchemes.FirstOrDefault(x => x.state == state).material;
            }
        }
    }
}