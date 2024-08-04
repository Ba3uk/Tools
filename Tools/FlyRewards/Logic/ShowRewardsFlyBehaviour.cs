using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace common.FlyRewards.Logic
{
    public sealed class ShowRewardsFlyBehaviour : MonoBehaviour
    {
        [Serializable]
        private sealed class RewardsFlyEffectPresetForDifferentQuantities
        {
            public int Count;
            public RewardsFlyEffectPreset RewardsFlyEffectPreset;
        }

        [SerializeField] private bool _isActive = true;


        [SerializeField] private bool _isOverrideRewardsFlyEffectPresetByCount;

        [SerializeField] private RewardsFlyEffectPreset _rewardsFlyEffectPreset;

        [SerializeField] //[ ShowIf(nameof(_isOverrideRewardsFlyEffectPresetByCount))]
        private List<RewardsFlyEffectPresetForDifferentQuantities> _listRewardsFlyEffectPreset;

        [SerializeField] //[Required, HideIf(nameof(_isOverrideStarPositionResource))]
        private Transform _startPosition;

        [Space(5)] [SerializeField] //[HideIf(nameof(HasResourceFlyEndPositionsInfo))][Required]
        private Transform _defaultEndPositionResource;

        //[ShowIf(nameof(HasResourceFlyEndPositionsInfo))] [ShowInInspector]
        [SerializeField] private RewardsFlyPositionsInfo _rewardsFlyPositionsInfo;

        [SerializeField] private GameObject[] _gameObjectsToActivateOnStart;
        [SerializeField] private GameObject[] _gameObjectsToActivateOnEnd;
        private RewardsFlyController _flyController;


        private readonly Dictionary<string, List<RewardsFly>> _listRewards = new();

        private readonly Dictionary<string, RewardsFly> _listRewardsForSequential = new();


        public void Initialize()
        {
            _rewardsFlyPositionsInfo = new RewardsFlyPositionsInfo(transform, _defaultEndPositionResource);
            InitializeController();
        }


        public void Initialize(Transform defaultEndPosition)
        {
            _rewardsFlyPositionsInfo = new RewardsFlyPositionsInfo(transform, defaultEndPosition);
            InitializeController();
        }

        public void Initialize(RewardsFlyPositionsInfo rewardsFlyPositionsInfo)
        {
            _rewardsFlyPositionsInfo = rewardsFlyPositionsInfo;
            InitializeController();
        }

        private void InitializeController()
        {
            _flyController = new RewardsFlyController();
            _flyController.SetElementCountThreshold(_rewardsFlyEffectPreset.MaxCountResourceFly);
        }

        public Sequence Show(IReadOnlyList<RewardsFly> rewardsFlies, Vector3 overrideStarPositionResource = default)
        {
            RewardsFlyEffectPreset rewardsFlyEffectPreset = GetRewardsFlyEffectPreset(rewardsFlies.Count);
            _flyController.SetPreset(rewardsFlyEffectPreset.name);
            var sequence = DOTween.Sequence();
            if (_isActive)
            {
                if (_rewardsFlyEffectPreset.IsSumCount)
                {
                    ShowSequential(rewardsFlies, sequence, overrideStarPositionResource);
                }
                else
                {
                    ShowParallel(rewardsFlies, sequence, overrideStarPositionResource);
                }
            }
            else
            {
                sequence.Complete();
            }

            return sequence;
        }

        private IRewardsFlyEffect CreateEffect(RewardsFly model, Vector3 overrideStarPositionResource)
        {
            var partsCount = model.Count;
            if (!_rewardsFlyEffectPreset.IsShowCount)
            {
                if (!_rewardsFlyEffectPreset.IsMaxCount)
                {
                    partsCount = _flyController.GetPartsCount(model.Count);
                }
            }

            var startPosition = _startPosition.position;
            return _flyController.CreateEffect(model.Sprite, partsCount, startPosition,
                _rewardsFlyPositionsInfo.Parent, _rewardsFlyEffectPreset.IsShowCount);
        }

        private void ShowSequential(
            IReadOnlyList<RewardsFly> rewardsFlies,
            Sequence sequence,
            Vector3 overrideStarPositionResource)
        {
            _listRewardsForSequential.Clear();
            foreach (var rewardsFly in rewardsFlies)
            {
                if (_listRewardsForSequential.ContainsKey(rewardsFly.Type))
                {
                    _listRewardsForSequential[rewardsFly.Type].Count += rewardsFly.Count;
                }
                else
                {
                    _listRewardsForSequential[rewardsFly.Type] = rewardsFly;
                }
            }

            var i = 0;
            foreach (RewardsFly model in _listRewardsForSequential.Values)
            {
                IRewardsFlyEffect resourceFlyEffect = CreateEffect(model, overrideStarPositionResource);
                var tween = resourceFlyEffect.Fly(GetStartPosition(model.Type));

                resourceFlyEffect.SetActive(false);
                sequence.InsertCallback(_rewardsFlyEffectPreset.DelayAfterAnimation * i,
                    () => resourceFlyEffect.SetActive(true));
                sequence.Insert(_rewardsFlyEffectPreset.DelayAfterAnimation * i, tween);
                tween.OnComplete(() =>
                {
                    resourceFlyEffect.SetActive(false);
                    foreach (GameObject o in _gameObjectsToActivateOnEnd)
                    {
                        o.SetActive(true);
                    }
                });
                i++;
            }
        }

        private void ShowParallel(
            IReadOnlyList<RewardsFly> rewardsFlies,
            Sequence sequence,
            Vector3 overrideStarPositionResource)
        {
            _listRewards.Clear();
            foreach (var rewardsFly in rewardsFlies)
            {
                if (_listRewards.ContainsKey(rewardsFly.Type))
                {
                    _listRewards[rewardsFly.Type].Add(rewardsFly);
                }
                else
                {
                    _listRewards[rewardsFly.Type] = new List<RewardsFly> {rewardsFly};
                }
            }

            var i = 0;
            foreach (var rewardType in _listRewards.Keys)
            {
                foreach (var model in _listRewards[rewardType])
                {
                    var resourceFlyEffect = CreateEffect(model, overrideStarPositionResource);
                    var tween = resourceFlyEffect.Fly(GetStartPosition(model.Type));

                    resourceFlyEffect.SetActive(false);
                    sequence.InsertCallback(_rewardsFlyEffectPreset.DelayAfterAnimation * i,
                        () => resourceFlyEffect.SetActive(true));
                    sequence.Insert(_rewardsFlyEffectPreset.DelayAfterAnimation * i, tween);
                    tween.OnComplete(() =>
                    {
                        resourceFlyEffect.SetActive(false);
                        foreach (GameObject o in _gameObjectsToActivateOnEnd)
                        {
                            o.SetActive(true);
                        }
                    });
                }

                i++;
            }
        }

        private Vector3 GetStartPosition(string type)
        {
            return _rewardsFlyPositionsInfo.DefaultEndPositionResource.position;
        }

        private RewardsFlyEffectPreset GetRewardsFlyEffectPreset(int rewardsFliesCount)
        {
            if (!_isOverrideRewardsFlyEffectPresetByCount)
            {
                return _rewardsFlyEffectPreset;
            }

            for (var i = 0; i < _listRewardsFlyEffectPreset.Count; i++)
            {
                if (_listRewardsFlyEffectPreset[i].Count > rewardsFliesCount)
                {
                    return _listRewardsFlyEffectPreset[i].RewardsFlyEffectPreset;
                }
            }

            return _listRewardsFlyEffectPreset[^1].RewardsFlyEffectPreset;
        }
    }
}