using Sanet.MVVM.Core.ViewModels;

namespace Sanet.MVVM.Core.Views;

public interface IBaseView
{
    object? ViewModel { get; set; }
}

public interface IBaseView<out TViewModel> : IBaseView where TViewModel : BaseViewModel
{
    new TViewModel? ViewModel { get; }
}
