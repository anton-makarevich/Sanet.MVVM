using Avalonia.Threading;
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.Core.ViewModels;
using Sanet.MVVM.Core.Views;

namespace Sanet.MVVM.Navigation.Avalonia.Services
{
    public abstract class BaseNavigationService : INavigationService
    {
        private readonly List<BaseViewModel> _viewModels = new();
        private readonly Dictionary<Type, Type> _viewModelViewDictionary = new();

        private readonly Stack<IBaseView> _backViewStack = new();
        private readonly IServiceProvider _container;


        protected BaseNavigationService(IServiceProvider container)
        {
            _container = container;
        }

        public void RegisterViews(Type view, Type viewModel)
        {
            //For now just manual registration
            _viewModelViewDictionary.Add(viewModel, view);
        }

        private T CreateViewModel<T>() where T : BaseViewModel
        {
            var vm = _container.GetService(typeof(T)) as T;
            vm?.SetNavigationService(this);
            _viewModels.Add(vm);
            return vm;
        }

        private Task OpenViewModelAsync<T>(T viewModel, bool modalPresentation = false)
            where T : BaseViewModel
        {
            return Dispatcher.UIThread.InvokeAsync(() =>
            {
                var view = CreateView(viewModel);
                var rootView = GetCurrentView();
                _backViewStack.Push(rootView);
                SetMainWindowContent(view);
            }).GetTask();
        }

        protected abstract IBaseView? GetCurrentView();
        protected abstract void SetMainWindowContent(IBaseView view);

        private IBaseView CreateView(BaseViewModel viewModel)
        {
            var viewModelType = viewModel.GetType();

            var viewType = _viewModelViewDictionary[viewModelType];

            var view = (IBaseView)Activator.CreateInstance(viewType);

            view.ViewModel = viewModel;

            return view;
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public T GetNewViewModel<T>() where T : BaseViewModel
        {
            var vm = (T)_viewModels.FirstOrDefault(f => f is T);

            if (vm != null)
            {
                _viewModels.Remove(vm);
            }

            vm = CreateViewModel<T>();
            _viewModels.Add(vm);
            return vm;
        }

        public T GetViewModel<T>() where T : BaseViewModel
        {
            var vm = (T)_viewModels.FirstOrDefault(f => f is T);
            if (vm == null)
            {
                vm = CreateViewModel<T>();
                _viewModels.Add(vm);
            }
            return vm;
        }

        public bool HasViewModel<T>() where T : BaseViewModel
        {
            var vm = (T)_viewModels.FirstOrDefault(f => f is T);
            return (vm != null);
        }

        public Task NavigateBackAsync()
        {
            return Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_backViewStack.Count > 0)
                {
                    var view = _backViewStack.Pop();
                    SetMainWindowContent(view);
                }
            }).GetTask();
        }

        public Task NavigateToRootAsync()
        {
            return Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_backViewStack.Count > 0)
                {
                    var view = _backViewStack.ToList()[_backViewStack.Count - 1];
                    SetMainWindowContent(view);
                }
            }).GetTask();
        }

        public Task NavigateToViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            return OpenViewModelAsync(viewModel);
        }

        public Task NavigateToViewModelAsync<T>() where T : BaseViewModel
        {
            var vm = GetViewModel<T>();
            if (vm == null)
                vm = CreateViewModel<T>();
            return OpenViewModelAsync(vm);
        }
        
        
        public Task ShowViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            return NavigateToViewModelAsync(viewModel);
        }

        public Task ShowViewModelAsync<T>() where T : BaseViewModel
        {
            return NavigateToViewModelAsync<T>();
        }

        public Task<TResult> ShowViewModelForResultAsync<T, TResult>(T viewModel)
            where T : BaseViewModel
            where TResult : class
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ShowViewModelForResultAsync<T, TResult>()
            where T : BaseViewModel
            where TResult : class
        {
            throw new NotImplementedException();
        }
    }
}
