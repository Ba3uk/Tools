using System;
using System.Collections.Generic;
using System.Linq;

namespace UnlocksModule.Logic
{
    public class CompositeUnlockFactory : ICompositeUnlockFactory
    {
        private readonly Dictionary<UnlockType, IUnlockFactory> factories;

        public CompositeUnlockFactory(List<IUnlockFactory> unlockFactories)
        {
            factories = new Dictionary<UnlockType, IUnlockFactory>();
            foreach (var factory in unlockFactories)
            {
                AddFactory(factory);
            }
        }

        public CompositeUnlocks CreateUnlocks(CompositeUnlockData compositeUnlockData)
        {
            List<IUnlock> unlocks = new List<IUnlock>(compositeUnlockData.UnlocksData.Count());

            foreach (var unlockData in compositeUnlockData.UnlocksData)
            {
                var unlock = factories[unlockData.Type].CreateUnlock(unlockData.Template);
                unlocks.Add(unlock);
            }

            return new CompositeUnlocks(unlocks);
        }

        private void AddFactory(IUnlockFactory factory)
        {
            foreach (var type in factory.SupportedTypes)
            {
                if (factories.ContainsKey(type))
                {
                    throw new Exception(
                        $"Can't add this factory, because the factory for this type has already been added. Type = {type} facotory = '{factories[type]}' ");
                }

                factories.Add(type, factory);
            }
        }
    }
}