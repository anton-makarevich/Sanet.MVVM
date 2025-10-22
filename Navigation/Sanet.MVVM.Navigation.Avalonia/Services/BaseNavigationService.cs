using Avalonia.Controls;
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
        var vm = _container.GetService(typeof(T)) as T;
        if (vm == null)
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

    public async Task<UiAction?> AskForActionAsync(string title, string description, params UiAction[] actions)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            // Get the current view to find the TopLevel
            var currentView = GetCurrentView();
            if (currentView is not Control control)
            {
                return null;
            }

            // Find the TopLevel (Window or similar)
            var topLevel = TopLevel.GetTopLevel(control);
            if (topLevel == null)
            {
                return null;
            }

            // Create the dialog
            var dialog = new ActionDialog();
            dialog.Initialize(title, description, actions);

            // Add the dialog to the visual tree
            // We need to add it as an overlay on the current content
            if (topLevel is Window { Content: Panel panel })
            {
                // Add dialog to existing panel
                panel.Children.Add(dialog);
                    
                // Wait for result
                var result = await dialog.GetResultAsync();
                    
                // Remove dialog from panel
                panel.Children.Remove(dialog);
                    
                return result;
            }

            if (topLevel is Window win)
            {
                // Wrap current content in a Grid and add dialog
                var originalContent = win.Content as Control;
                if (originalContent == null)
                {
                    return null;
                }
                var grid = new Grid();
                grid.Children.Add(originalContent);
                grid.Children.Add(dialog);
                win.Content = grid;
                    
                // Wait for result
                var result = await dialog.GetResultAsync();
                    
                // Restore original content
                win.Content = originalContent;
                    
                return result;
            }

            return null;
        });
    }
}