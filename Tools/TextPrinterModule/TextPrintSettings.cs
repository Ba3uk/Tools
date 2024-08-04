using DG.Tweening;
using TMPro;
using UnityEngine;

namespace common.TextPrinter
{
    public sealed class TextPrintSettings : MonoBehaviour
    {
        public int MaxVisibleCharacters = 9999;
        public TMP_Text MessageTF;
        public Ease TextTypingEase = Ease.Linear;
        public int SymbolsPerSecond = 25;

        [Range(2, 30)] public float AccelerationFactor;
    }
}