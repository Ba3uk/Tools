using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace UnlocksModule.Logic
{
    public class CompositeUnlocks : IUnlock, IDisposable
    {
        private readonly List<IUnlock> unlocks;
        private readonly List<IDisposable> disposables;
        private readonly ReactiveProperty<bool> isUnlock = new();

        public IReadonlyReactiveProperty<bool> IsUnlock => isUnlock;


        public CompositeUnlocks(List<IUnlock> unlocks)
        {
            this.unlocks = unlocks;
            disposables = new(unlocks.Count());

            foreach (var unlock in unlocks)
            {
                disposables.Add(unlock.IsUnlock.SubscribeChanged(OnChangeValue));
            }

            UpdateValue();
        }

        private void OnChangeValue(bool obj)
        {
            UpdateValue();
        }

        private void UpdateValue()
        {
            bool isUnlocked = true;
            foreach (var unlock in unlocks)
            {
                if (!unlock.IsUnlock.Value)
                {
                    isUnlocked = false;
                    break;
                }
            }

            isUnlock.Value = isUnlocked;
        }

        public void Dispose()
        {
            disposables.ForEach(d => d.Dispose());
        }
    }
}