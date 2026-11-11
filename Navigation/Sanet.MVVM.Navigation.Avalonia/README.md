# Sanet.MVVM.Navigation.Avalonia

Avalonia v12 implementations of `INavigationService` for classic desktop and single-view application
lifetimes.

## Installation

```bash
dotnet add package Sanet.MVVM.Navigation.Avalonia
```

## Target framework and dependencies

- Target framework: `net10.0`
- `Avalonia` 12.1.6
- `Sanet.MVVM.Core`

## Minimal usage

Classic desktop lifetime (`IClassicDesktopStyleApplicationLifetime`):

```csharp
using Sanet.MVVM.Navigation.Avalonia.Services;

var navigation = new NavigationService(desktopLifetime, serviceProvider);

navigation.RegisterViews(typeof(MainView), typeof(MainViewModel));
await navigation.ShowViewModelAsync<MainViewModel>();
```

Single-view lifetime (`ISingleViewApplicationLifetime`):

```csharp
var navigation = new SingleViewNavigationService(
    singleViewLifetime,
    contentControl,
    serviceProvider);

navigation.RegisterViews(typeof(MainView), typeof(MainViewModel));
```

## Prerequisites

- The host must already show an initial `IBaseView`. `NavigationService` reads and writes
  `IClassicDesktopStyleApplicationLifetime.MainWindow.Content`.
- `SingleViewNavigationService` assigns the supplied `ContentControl` to
  `ISingleViewApplicationLifetime.MainView` and reads and writes that control's `Content`.
- Register each pair with `RegisterViews(viewType, viewModelType)`. The view type comes first.
- Views must have a public parameterless constructor because the service creates them with
  `Activator.CreateInstance`.
- View models must be resolvable from the supplied `IServiceProvider` for the parameterless
  navigation overloads, for example `ShowViewModelAsync<T>()`.
- The application owns the `IServiceProvider`. `Sanet.MVVM.DI.Avalonia` stores an
  `IServiceCollection` but does not build a provider.

## Behavior

- `ShowViewModelAsync(viewModel)` and `ShowViewModelAsync<T>()` perform ordinary navigation: the
  current view is pushed onto a back stack and replaced as the main content. This is not a modal
  dialog.
- `NavigateBackAsync()` pops the back stack and restores the previous view.
- `NavigateToRootAsync()` restores the bottom view of the back stack.
- `ShowViewModelForResultAsync<T, TResult>(viewModel)` displays the view in Avalonia's
  `OverlayLayer`, waits for `IResultProvider<TResult>.GetResultAsync()`, and returns its result.
  It requires the current view to be an Avalonia `Control` with a `TopLevel`.
- `AskForActionAsync(title, description, actions)` shows an overlay dialog and returns the selected
  `UiAction`. It does not execute the action's `Command`; the caller decides what to do.
- UI work is dispatched through `Dispatcher.UIThread`.

## Notes and limitations

- `CloseAsync()` is not implemented and throws `NotImplementedException`.
- The navigation service creates and caches view models. A view-model type that the provider cannot
  resolve makes the parameterless navigation overloads throw `InvalidOperationException`.
- Register each view/view-model mapping once; `RegisterViews` uses a dictionary `Add` and duplicate
  registrations throw.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
