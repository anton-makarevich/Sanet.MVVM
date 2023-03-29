using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Sanet.MVVM.Core.Services;

namespace Sanet.MVVM.Core.ViewModels;

public abstract class BaseViewModel : BindableBase
{
    public event EventHandler<object?>? OnResult;

    #region Fields
    private bool _isBusy;
    private INavigationService? _navigationService;
    #endregion
    protected BaseViewModel() { }

    protected BaseViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    #region Properties

    // ReSharper disable once LocalizableElement
    public INavigationService NavigationService => _navigationService ?? throw new ArgumentNullException(
        nameof(NavigationService),
        "Navigation service should be initialized");

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public bool ExpectsResult { get; set; }
    #endregion

    #region Commands
    public ICommand BackCommand => new AsyncCommand(GoBackAsync);
    #endregion

    #region Methods
    public void SetNavigationService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    private Task GoBackAsync()
    {
        return NavigationService.NavigateBackAsync();
    }

    internal async Task CloseAsync(object? result = null)
    {
        await NavigationService.CloseAsync();
        if (ExpectsResult)
        {
            ExpectsResult = false;
            OnResult?.Invoke(this, result);
        }
    }

    public virtual void AttachHandlers() { }

    public virtual void DetachHandlers() { }

    #endregion
}
