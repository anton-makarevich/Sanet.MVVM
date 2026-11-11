# Sanet.MVVM.Core

MVVM base classes and contracts for .NET 10.

## Installation

```bash
dotnet add package Sanet.MVVM.Core
```

## Minimal usage

```csharp
using Sanet.MVVM.Core.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private string _title = "Hello";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public override void AttachHandlers()
    {
        // Subscribe to services or events.
    }

    public override void DetachHandlers()
    {
        // Unsubscribe here.
    }
}
```

## `BindableBase`

`BindableBase` is an abstract base class that implements `INotifyPropertyChanged`. It exposes the
following members to derived classes:

- `NotifyPropertyChanged([CallerMemberName] string? propertyName = null)` raises `PropertyChanged`.
  It is `protected virtual`.
- `SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)`
  compares the current and new value with `Equals`, assigns the field, and raises `PropertyChanged`
  only when the value changes.
- `NotifyAllPropertiesChanged()` raises `PropertyChanged` with an empty property name so that all
  bindings refresh.

## `BaseViewModel`

`BaseViewModel` is abstract and derives from `BindableBase`.

- `IsBusy` is a bindable busy flag.
- `BackCommand` is an `ICommand` that calls `INavigationService.NavigateBackAsync`.
- `NavigationService` returns the navigation service supplied to the constructor or to
  `SetNavigationService`. Accessing it before initialization throws `ArgumentNullException`.
- Two protected constructors are available: a parameterless one and
  `BaseViewModel(INavigationService navigationService)`.
- `SetNavigationService(INavigationService navigationService)` initializes or replaces the navigation
  service after construction.
- `AttachHandlers()` and `DetachHandlers()` are virtual hooks for subscribing and unsubscribing from
  services or events. The Avalonia base view calls them when the view enters or leaves the visual tree.

`BaseViewModel` does not implement `IDisposable` or `IAsyncDisposable`. Derived view models may add
either interface when they need to release resources.

## Contracts

- `IBaseView` exposes an `object? ViewModel { get; set; }` property.
- `IBaseView<out TViewModel>` is the typed variant used by `BaseView<TViewModel>`.
- `INavigationService` defines navigation, back navigation, result-returning overlays, view
  registration, and `AskForActionAsync`.
- `IResultProvider<TResult>` defines `Task<TResult> GetResultAsync()` for view models that return a
  result.
- `UiAction` is a record with `Title`, an optional `Command`, and an optional `CommandParameter`.
- `IExternalNavigationService` defines `OpenUrlAsync(string url)` and
  `OpenEmailAsync(string emailAddress, string subject)`.

## Notes and limitations

- `Sanet.MVVM.Core` does not create a dependency-injection container and does not provide a navigation
  implementation. Use `Sanet.MVVM.Navigation.Avalonia` for Avalonia, or implement
  `INavigationService` yourself.
- Always initialize `NavigationService` before a view model uses navigation.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
