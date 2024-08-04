using UnityEngine;
using UnityEngine.UI;

namespace common.FlyRewards.Logic
{
    internal sealed class RewardsFlyBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private Sprite sprite;

        public Sprite Sprite
        {
            get => sprite;
            set
            {
                sprite = value;
                _image.sprite = sprite;
            }
        }

        public int Width
        {
            get => (int) _image.rectTransform.sizeDelta.x;
            set => _image.rectTransform.sizeDelta = new Vector2(value, _image.rectTransform.sizeDelta.y);
        }

        public int Height
        {
            get => (int) _image.rectTransform.sizeDelta.y;
            set => _image.rectTransform.sizeDelta = new Vector2(_image.rectTransform.sizeDelta.x, value);
        }
    }
}