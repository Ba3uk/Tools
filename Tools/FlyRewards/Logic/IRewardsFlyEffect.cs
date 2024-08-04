using DG.Tweening;
using UnityEngine;

namespace common.FlyRewards.Logic
{
    public interface IRewardsFlyEffect
    {
        Tween Fly(Vector3 endPos, Vector2? scale = null);
        void SetActive(bool value);
    }
}
