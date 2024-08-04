using System;
using UnityEngine;

namespace common.FlyRewards.Logic
{
    [Serializable]
    public sealed class RewardsFlyPositionsInfo
    {
        [SerializeField] private Transform _parent;

        public Transform Parent => _parent;

        [Space(5)] [SerializeField] private Transform _defaultEndPositionResource;

        public Transform DefaultEndPositionResource => _defaultEndPositionResource;

        public RewardsFlyPositionsInfo()
        {
        }

        public RewardsFlyPositionsInfo(Transform parent, Transform defaultEndPositionResource)
        {
            _parent = parent;
            _defaultEndPositionResource = defaultEndPositionResource;
        }
    }

    public class FlyPosition
    {
        public string Key;
        public Transform Transform;
    }
}