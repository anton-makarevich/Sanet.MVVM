using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Sanet.MVVM.Core.Views;

namespace Sanet.MVVM.Navigation.Avalonia.Services;

public class SingleViewNavigationService : BaseNavigationService
{

    private readonly ContentControl _mainView;
    
    public SingleViewNavigationService(ISingleViewApplicationLifetime singleViewPlatform, ContentControl contentControl,
        IServiceProvider container) : base(container)
    {
        singleViewPlatform.MainView = contentControl;
        _mainView = contentControl;
    }

    protected override IBaseView? GetCurrentView()
    {
        return _mainView.Content as IBaseView;
    }

    protected override void SetMainWindowContent(IBaseView view)
    {
        _mainView.Content = view;
    }
}