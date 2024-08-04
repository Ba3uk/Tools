using System;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace common.TextPrinter
{
    public sealed class TextPrintViewModel : IDisposable
    {
        private readonly string message;
        private Tween _typingTween;
        private readonly TextPrintSettings _settingsView;
        private readonly ReactiveProperty<bool> isComplete = new();
        public IReadonlyReactiveProperty<bool> IsComplete => isComplete;

        public TextPrintViewModel(TextPrintSettings settingsView, string message)
        {
            _settingsView = settingsView;
            this.message = message;
        }

        public void ShowImmediately()
        {
            _settingsView.MessageTF.text = message;
            _settingsView.MessageTF.ForceMeshUpdate();
            isComplete.Value = true;
        }

        public void SkipAnimation()
        {
            if (_typingTween == null)
            {
                return;
            }

            _typingTween.timeScale = _settingsView.AccelerationFactor;
        }

        public async UniTask ShowAsync()
        {
            isComplete.Value = false;
            var completionSource = new UniTaskCompletionSource();
            _typingTween?.Kill();
            _settingsView.MessageTF.text = message;
            _settingsView.MessageTF.ForceMeshUpdate();
            int totalCharacters = _settingsView.MessageTF.text.Length;
            _settingsView.MessageTF.maxVisibleCharacters = 0;
            _typingTween = DOTween
                .To(getter: () => _settingsView.MessageTF.maxVisibleCharacters,
                    setter: (visibleChars) => { _settingsView.MessageTF.maxVisibleCharacters = visibleChars; },
                    endValue: totalCharacters,
                    duration: message.Length / (float) _settingsView.SymbolsPerSecond)
                .SetEase(_settingsView.TextTypingEase)
                .OnComplete(() =>
                {
                    _settingsView.MessageTF.maxVisibleCharacters = _settingsView.MaxVisibleCharacters;
                    completionSource.TrySetResult();
                    _typingTween = null;
                    isComplete.Value = true;
                });

            await completionSource.Task;
        }

        public void ForceFinish()
        {
            _typingTween?.Kill(true);
        }

        public void Dispose()
        {
            _typingTween?.Kill();
        }
    }
}