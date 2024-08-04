using DG.Tweening;
using UnityEngine;

namespace common.FlyRewards.Logic
{
    [CreateAssetMenu(fileName = "RewardsFlyPreset", menuName = "RewardsFlyPreset", order = 30)]
    public sealed class RewardsFlyEffectPreset : ScriptableObject
    {
        public Vector2 Size;
        [Tooltip("if 0 - Lanes = Count")] public int Lanes = 0;
        public float MaxRadius = 200;
        public float MinRadius = 100;
        public float Step1 = 0.15f;
        public Ease Step1Ease = Ease.OutCubic;
        public Vector2 PeakMult = new Vector2(0.66f, 1.5f);
        public float Step1DiffMin = 0.8f;
        public float Step1ItemDelay = 0.02f;
        public float Step2 = 0.1f;
        public Ease Step2Ease = Ease.InCubic;
        public float BounceHalftime = 0.05f;
        public float Delay = 0.2f;
        public float FlyTime = 0.3f;
        public Ease FlyEase = Ease.InCubic;
        public bool IgnoreDoubleTraceFly = false;
        public bool EnableRotation = false;
        public bool NegateRotationIfXIsLowerThanZero = false;

        [SerializeField] //, ShowIf(nameof(NeedShowCount))]
        public bool IsShowCount;

        [SerializeField] //, ShowIf(nameof(NeedShowCount))]
        public float DelayAfterAnimation = 0.3f;

        [SerializeField] //, ShowIf(nameof(NeedShowCount))]
        public bool IsSumCount;

        [SerializeField] //, ShowIf(nameof(NeedShowMaxCount))]
        public bool IsMaxCount;

        [SerializeField] //, Min(1), ShowIf(nameof(NeedShowMaxCountResourceFly))]
        public int MaxCountResourceFly = 10;

        public AnimationCurve XRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve YRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve ZRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private bool NeedShowMaxCountResourceFly()
        {
            return !IsShowCount && !IsMaxCount;
        }

        private bool NeedShowMaxCount()
        {
            return !IsShowCount;
        }

        private bool NeedShowCount()
        {
            return !IsMaxCount;
        }
    }
}