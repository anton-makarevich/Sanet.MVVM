# Sanet.MVVM.Views.Avalonia

Typed Avalonia v12 base view that connects a view model to a `UserControl`.

## Installation

```bash
dotnet add package Sanet.MVVM.Views.Avalonia
```

## Target framework and dependencies

- Target framework: `net10.0`
- `Avalonia` 12.1.6
- `Sanet.MVVM.Core`

## Minimal usage

```csharp
using Sanet.MVVM.Views.Avalonia;

public partial class MainView : BaseView<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnViewModelSet()
    {
        // Called after ViewModel and DataContext are assigned.
    }
}
```

The view's XAML can use ordinary bindings because `BaseView<TViewModel>` assigns the view model to
`DataContext`.

## Behavior

- `BaseView<TViewModel>` derives from Avalonia `UserControl` and implements
  `IBaseView<TViewModel>`.
- Setting the `ViewModel` property stores the value, assigns it to `DataContext`, and calls the
  virtual `OnViewModelSet()` hook.
- The explicit `IBaseView.ViewModel` implementation forwards values to the typed `ViewModel`
  property.
- `OnAttachedToVisualTree` calls `ViewModel?.AttachHandlers()`.
- `OnDetachedFromVisualTree` calls `ViewModel?.DetachHandlers()`.

## Notes and limitations

- The navigation service creates views with `Activator.CreateInstance`, so a view used for navigation
  needs a public parameterless constructor.
- `DataContext` and the `ViewModel` property always point at the same instance after assignment.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
