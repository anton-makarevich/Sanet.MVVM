using Avalonia;
using Avalonia.Controls;
using Sanet.MVVM.Core.ViewModels;
using Sanet.MVVM.Core.Views;

namespace Sanet.MVVM.Views.Avalonia;

public abstract class BaseView<TViewModel> : UserControl,IBaseView<TViewModel> where TViewModel : BaseViewModel
{
    protected bool NavigationBarEnabled;

    private TViewModel? _viewModel;

    public virtual TViewModel? ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            DataContext = _viewModel;
            OnViewModelSet();
        }
    }

    object? IBaseView.ViewModel
    {
        get => _viewModel;
        set => ViewModel = (TViewModel?)value;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ViewModel?.AttachHandlers();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ViewModel?.DetachHandlers();
    }

    protected virtual void OnViewModelSet() { }
}
