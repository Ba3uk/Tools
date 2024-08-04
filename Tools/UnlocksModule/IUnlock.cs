using System;
using Common;

namespace UnlocksModule.Logic
{
    public interface IUnlock : IDisposable
    {
        IReadonlyReactiveProperty<bool> IsUnlock { get; }
    }
}