using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnlocksModule.Logic
{
    [Serializable]
    public class CompositeUnlockData
    {
        [SerializeField] private List<UnlockData> unlocksData;
        public IReadOnlyList<UnlockData> UnlocksData => unlocksData;
    }
}