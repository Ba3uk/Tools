using System;
using UnityEngine;

namespace UnlocksModule.Logic
{
    [Serializable]
    public class UnlockData
    {
        [field: SerializeField] public UnlockType Type { get; private set; }
        [field: SerializeField] public string Template { get; private set; }
    }
}