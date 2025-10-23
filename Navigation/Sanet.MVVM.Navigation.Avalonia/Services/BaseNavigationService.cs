using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Sanet.MVVM.Core.Models;
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.Core.ViewModels;
using Sanet.MVVM.Core.Views;
using Sanet.MVVM.Navigation.Avalonia.Views;

namespace Sanet.MVVM.Navigation.Avalonia.Services;

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

    private T? CreateViewModel<T>() where T : BaseViewModel
    {
        if (_container.GetService(typeof(T)) is not T vm)
            return null;
        vm.SetNavigationService(this);
        _viewModels.Add(vm);
        return vm;
    }

    private Task OpenViewModelAsync<T>(T viewModel)
        where T : BaseViewModel
    {
        return Dispatcher.UIThread.InvokeAsync(() =>
        {
            var view = CreateView(viewModel);
            var rootView = GetCurrentView();
            if (view == null || rootView == null) return;
            _backViewStack.Push(rootView);
            SetMainWindowContent(view);
        }).GetTask();
    }

    protected abstract IBaseView? GetCurrentView();
    protected abstract void SetMainWindowContent(IBaseView view);

    private IBaseView? CreateView(BaseViewModel viewModel)
    {
        var viewModelType = viewModel.GetType();

        var viewType = _viewModelViewDictionary[viewModelType];

        var view = (IBaseView?)Activator.CreateInstance(viewType);
            
        if (view == null)
            return null;

        view.ViewModel = viewModel;

        return view;
    }

    public Task CloseAsync()
    {
        throw new NotImplementedException();
    }

    public T? GetNewViewModel<T>() where T : BaseViewModel
    {
        var vm = (T?)_viewModels.FirstOrDefault(f => f is T);

        if (vm != null)
        {
            _viewModels.Remove(vm);
            // ReSharper disable once SuspiciousTypeConversion.Global
            (vm as IDisposable)?.Dispose();
        }

        vm = CreateViewModel<T>();
        if (vm == null)
            return null;
            
        _viewModels.Add(vm);
        return vm;
    }

    public T? GetViewModel<T>() where T : BaseViewModel
    {
        var vm = (T?)_viewModels.FirstOrDefault(f => f is T);
        if (vm != null) return vm;
        vm = CreateViewModel<T>();
        if (vm == null)
            return null;
        _viewModels.Add(vm);
        return vm;
    }

    public bool HasViewModel<T>() where T : BaseViewModel
    {
        var vm = (T?)_viewModels.FirstOrDefault(f => f is T);
        return vm != null;
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
        var vm = GetViewModel<T>() ?? CreateViewModel<T>();
        return vm == null 
            ? throw new InvalidOperationException($"ViewModel of type {typeof(T)} is not registered") 
            : OpenViewModelAsync(vm);
    }
        
        
    public Task ShowViewModelAsync<T>(T viewModel) where T : BaseViewModel
    {
        return NavigateToViewModelAsync(viewModel);
    }

    public Task ShowViewModelAsync<T>() where T : BaseViewModel
    {
        return NavigateToViewModelAsync<T>();
    }

    public async Task<TResult> ShowViewModelForResultAsync<T, TResult>(T viewModel)
        where T : BaseViewModel, IResultProvider<TResult>
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var currentView = GetCurrentView();
            if (currentView is not Control currentViewControl)
                throw new InvalidOperationException("Unable to find current view");

            // Find the TopLevel
            var topLevel = TopLevel.GetTopLevel(currentViewControl);
            if (topLevel == null)
                throw new InvalidOperationException("Unable to find TopLevel");

            // Create the view for the viewmodel
            var view = CreateView(viewModel);
            if (view is not Control viewControl)
                throw new InvalidOperationException($"View for {typeof(T)} is not a Control");

            // Find OverlayLayer
            var overlayLayer = OverlayLayer.GetOverlayLayer(topLevel);
            if (overlayLayer == null)
                throw new InvalidOperationException("Unable to find OverlayLayer for hosting the view");

            viewControl.MaxHeight = topLevel.Height-20;
            viewControl.MaxWidth = topLevel.Width-40;
            
            // Prepare hosting container
            var host = CreateHostPanel(topLevel);
            host.Children.Add(viewControl);

            // Add to overlay
            overlayLayer.Children.Add(host);

            try
            {
                // Wait for result from viewmodel
                var result = await viewModel.GetResultAsync();
                return result;
            }
            finally
            {
                // Cleanup
                overlayLayer.Children.Remove(host);
            }
        });
    }

    public Task<TResult> ShowViewModelForResultAsync<T, TResult>()
        where T : BaseViewModel, IResultProvider<TResult>
    {
        var vm = GetViewModel<T>() ?? CreateViewModel<T>();
        return vm == null 
            ? throw new InvalidOperationException($"ViewModel of type {typeof(T)} is not registered") 
            : ShowViewModelForResultAsync<T, TResult>(vm);
    }

    public async Task<UiAction?> AskForActionAsync(string title, string description, params UiAction[] actions)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            // Get the current view to find the TopLevel
            var currentView = GetCurrentView();
            if (currentView is not Control control)
                return null;

            // Find the TopLevel
            var topLevel = TopLevel.GetTopLevel(control);
            if (topLevel == null)
                return null;

            // Create the dialog
            var dialog = new ActionDialog();
            dialog.Initialize(title, description, actions);

            // Find OverlayLayer (every TopLevel usually has one)
            var overlayLayer = OverlayLayer.GetOverlayLayer(topLevel);
            if (overlayLayer == null)
                throw new InvalidOperationException("Unable to find OverlayLayer for hosting the dialog");

            // Prepare hosting container
            var host = CreateHostPanel(topLevel);
            host.Children.Add(dialog);

            // Add to overlay
            overlayLayer.Children.Add(host);

            try
            {
                // Wait for result from dialog
                var result = await dialog.GetResultAsync();
                return result;
            }
            finally
            {
                // Cleanup
                overlayLayer.Children.Remove(host);
            }
        });
    }

    private Panel CreateHostPanel(TopLevel topLevel)
    {
        return new Panel
        {
            Height = topLevel.Height,
            Width = topLevel.Width,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
            IsHitTestVisible = true
        };
    }
}

