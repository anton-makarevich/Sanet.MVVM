using Avalonia.Controls;
using Avalonia.Interactivity;
using Sanet.MVVM.Core.Models;

namespace Sanet.MVVM.Navigation.Avalonia.Views;

public partial class ActionDialog : UserControl
{
    private readonly TaskCompletionSource<UiAction?> _taskCompletionSource = new();

    public ActionDialog()
    {
        InitializeComponent();
    }

    public void Initialize(string title, string description, UiAction[] actions)
    {
        TitleTextBlock.Text = title;
        DescriptionTextBlock.Text = description;
        ActionsItemsControl.ItemsSource = actions;
    }

    public Task<UiAction?> GetResultAsync()
    {
        return _taskCompletionSource.Task;
    }

    private void ActionButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: UiAction action })
        {
            _taskCompletionSource.TrySetResult(action);
        }
    }
}
