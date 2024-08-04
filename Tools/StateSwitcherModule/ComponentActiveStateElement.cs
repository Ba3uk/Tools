using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateScripts
{
    internal abstract class ComponentActiveStateElement<T> : StateElement where T : Behaviour
    {
        [Serializable]
        private class ContainerStates<T>
        {
            public List<string> States;
            public T Behaviour;
        }

        [SerializeField] private List<ContainerStates<T>> _containers;

        public override void SetState(string state)
        {
            foreach (var container in _containers)
            {
                container.Behaviour.enabled = container.States.Contains(state);
            }
        }
    }
}
