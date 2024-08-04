using UnityEngine;

namespace StateScripts
{
    public abstract class StateElement : MonoBehaviour
    {
        public abstract void SetState(string state);
    }
}