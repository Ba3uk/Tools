using System;

namespace UnlocksModule.Logic
{
    public interface IUnlockFactory : IDisposable
    {
        UnlockType[] SupportedTypes { get; }
        IUnlock CreateUnlock(string template);
    }
}