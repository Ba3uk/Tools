#if ZENJECT
using Common;
using Patterns.MVVM;
using Zenject;

namespace ViewModelExtension
{
    public abstract class ViewModel<TViewModel, TModel> : DisposableObject, IViewModel where TViewModel : class
    {
        protected TModel Args { get; }

        public ViewModel(TModel args)
        {
            Args = args;
        }

        public class Factory : PlaceholderFactory<TModel, TViewModel>
        {
        }
    }

}
#endif