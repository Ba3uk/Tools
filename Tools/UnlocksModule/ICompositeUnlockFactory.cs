namespace UnlocksModule.Logic
{
    public interface ICompositeUnlockFactory
    {
        CompositeUnlocks CreateUnlocks(CompositeUnlockData compositeUnlockData);
    }
}