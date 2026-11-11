# Sanet.MVVM

MVVM contracts and optional Avalonia UI v12 building blocks for .NET 10.

The packages provide base classes, navigation abstractions, an Avalonia base view, a lightweight
Microsoft dependency-injection hook, and platform-specific external navigation helpers. They are not a
complete application bootstrap: you supply the `IServiceProvider`, choose a navigation service, register
view models and view mappings, and install an initial `IBaseView`.

## Packages

| Package | Purpose |
| --- | --- |
| [Sanet.MVVM.Core](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/Sanet.MVVM.Core/README.md) | MVVM base classes and contracts (`BindableBase`, `BaseViewModel`, `INavigationService`, `IBaseView`, `UiAction`, `IExternalNavigationService`) |
| [Sanet.MVVM.Views.Avalonia](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/Views/Sanet.MVVM.Views.Avalonia/README.md) | Typed Avalonia base view (`BaseView<TViewModel>`) |
| [Sanet.MVVM.DI.Avalonia](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/DI/Sanet.MVVM.DI.Avalonia/README.md) | `AppBuilder` hook that stores an `IServiceCollection` in application resources |
| [Sanet.MVVM.Navigation.Avalonia](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/Navigation/Sanet.MVVM.Navigation.Avalonia/README.md) | `INavigationService` implementations for Avalonia desktop and single-view lifetimes |
| [Sanet.MVVM.ExternalNavigation.Android](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/ExternalNavigation/Sanet.MVVM.ExternalNavigation.Android/README.md) | Android implementation of `IExternalNavigationService` using intents |
| [Sanet.MVVM.ExternalNavigation.Browser](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/ExternalNavigation/Sanet.MVVM.ExternalNavigation.Browser/README.md) | Browser (WASM) implementation of `IExternalNavigationService` using `window.open` |
| [Sanet.MVVM.ExternalNavigation.Desktop](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/ExternalNavigation/Sanet.MVVM.ExternalNavigation.Desktop/README.md) | Desktop implementation of `IExternalNavigationService` using OS-specific shell commands |
| [Sanet.MVVM.ExternalNavigation.iOS](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/ExternalNavigation/Sanet.MVVM.ExternalNavigation.iOS/README.md) | iOS implementation of `IExternalNavigationService` using UIKit |

## Installation

Install only the packages you need:

```bash
dotnet add package Sanet.MVVM.Core
dotnet add package Sanet.MVVM.Views.Avalonia
dotnet add package Sanet.MVVM.DI.Avalonia
dotnet add package Sanet.MVVM.Navigation.Avalonia
dotnet add package Sanet.MVVM.ExternalNavigation.Android
dotnet add package Sanet.MVVM.ExternalNavigation.Browser
dotnet add package Sanet.MVVM.ExternalNavigation.Desktop
dotnet add package Sanet.MVVM.ExternalNavigation.iOS
```

## How the packages fit together

1. The application registers view models and any platform services, such as an
   `IExternalNavigationService`.
2. The application builds and owns the `IServiceProvider`.
3. The application creates `NavigationService` (classic desktop lifetime) or
   `SingleViewNavigationService` (single-view lifetime).
4. The application calls `RegisterViews(viewType, viewModelType)` for each view/view-model pair.
5. The host shows an initial `IBaseView` so the navigation service can find the current view and its
   `TopLevel`.
6. View models navigate with `ShowViewModelAsync`, `ShowViewModelForResultAsync`, or
   `AskForActionAsync`.

`Sanet.MVVM.DI.Avalonia` only stores the `IServiceCollection` in Avalonia application resources. It does
not build the provider and does not register navigation; the application still has to do both.

## Build and test

The libraries target .NET 10.

```bash
dotnet build Sanet.MVVM.sln
dotnet test Sanet.MVVM.sln
```

The Android, Browser, and iOS projects need the corresponding workloads:

```bash
dotnet workload install android wasm-tools ios
```

The iOS project must be packed on macOS. `.github/workflows/publish-nugets.yml` packs and publishes the
packages.

## License

This project is licensed under the MIT License. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
