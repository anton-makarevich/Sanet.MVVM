using Avalonia.Controls.ApplicationLifetimes;
using Sanet.MVVM.Core.Views;

namespace Sanet.MVVM.Navigation.Avalonia.Services;

public class NavigationService : BaseNavigationService
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktop;
    
    public NavigationService(IClassicDesktopStyleApplicationLifetime desktop, IServiceProvider container):base(container)
    {
        _desktop = desktop;
    }

    protected override IBaseView? GetCurrentView()
    {
        return _desktop?.MainWindow?.Content as IBaseView;
    }

    protected override void SetMainWindowContent(IBaseView view)
    {
        _desktop.MainWindow.Content = view;
    }
}
