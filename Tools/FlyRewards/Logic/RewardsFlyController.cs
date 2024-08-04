using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace common.FlyRewards.Logic
{
    public sealed class RewardsFlyController
    {
        private const string RESOURCE_FLY_EFFECT_PATH = "ResourceFly/ResourceFlyEffect";
        private const string RESOURCE_FLY_EFFECT_PRESET_PATH = "ResourceFly/ResourceFlyAnimationPresets/";
        private static readonly List<RewardsFlyEffectPreset> _presets = new();
        private static readonly RewardsFlyEffect RewardsFlyEffect;
        
        private int _elementCountThreshold;
        private RewardsFlyEffectPreset _preset;

        static RewardsFlyController()
        {
            RewardsFlyEffect = Resources.Load<RewardsFlyEffect>(RESOURCE_FLY_EFFECT_PATH);
            _presets.AddRange(Resources.LoadAll<RewardsFlyEffectPreset>(RESOURCE_FLY_EFFECT_PRESET_PATH));
        }

        public IRewardsFlyEffect CreateEffect(Sprite sprite, int count, Vector3 start, Transform parent,
            bool isShowCount, Action startResourceCollect = null)
        {
            start.z = -20;
            var rewEffect = InstantiateRewardEffect(start, parent);
            rewEffect.FirstComplete = startResourceCollect;
            rewEffect.IsShowCount = isShowCount;
            rewEffect.Prepare(sprite, count, _preset);
            return rewEffect;
        }

        public int GetPartsCount(int count, int lowestResourceAmount = 0)
        {
            var partCount = Mathf.CeilToInt(Mathf.Sqrt(count) / 2);
            if (partCount > _elementCountThreshold)
            {
                partCount = _elementCountThreshold + Mathf.CeilToInt(Mathf.Sqrt(partCount - _elementCountThreshold));
            }

            if (lowestResourceAmount > 0 && partCount < lowestResourceAmount)
            {
                partCount = lowestResourceAmount;
            }

            return partCount;
        }

        public void SetElementCountThreshold(int elementCountThreshold)
        {
            _elementCountThreshold = elementCountThreshold;
        }

        public void SetPreset(string presetName)
        {
            _preset = _presets.Find(a => a.name == presetName) ??
                      _presets.Find(a => a.name == "Default");
        }

        private RewardsFlyEffect InstantiateRewardEffect(Vector3 start, Transform parent = null)
        {
            var effect = Object.Instantiate(RewardsFlyEffect, parent);
            effect.transform.position = start;
            return effect;
        }
    }
}