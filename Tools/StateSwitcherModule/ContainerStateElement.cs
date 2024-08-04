using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateScripts
{
    public class ContainerStateElement : StateElement
    {
        [Serializable]
        private class ContainerStates
        {
            public List<string> States;
            public GameObject Container;
        }

        [SerializeField] private List<ContainerStates> _containers;

        public override void SetState(string state)
        {
            foreach (var container in _containers)
            {
                container.Container.SetActive(container.States.Contains(state));
            }
        }
    }
}