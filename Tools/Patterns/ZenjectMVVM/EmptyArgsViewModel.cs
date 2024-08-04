#if ZENJECT
using Common;
using Patterns.MVVM;
using Zenject;

namespace ViewModelExtension
{
    public abstract class EmptyArgsViewModel<TViewModel> : DisposableObject, IViewModel
    {
        public class Factory : PlaceholderFactory<TViewModel>
        {
        }
    }
}

#endif