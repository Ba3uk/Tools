using UnityEngine;

namespace common.FlyRewards.Logic
{
    public sealed class RewardsFly 
    {
        public int Count { get; set; }
        public Sprite Sprite { get; }
        public string Type { get; }
        
        public RewardsFly(int count, Sprite sprite, string type)
        {
            Count = count;
            Sprite = sprite;
            Type = type;
        }
    }
}