# Sanet.MVVM.ExternalNavigation.Android

Android implementation of `IExternalNavigationService` that opens URLs and email clients using Android
intents.

## Installation

```bash
dotnet add package Sanet.MVVM.ExternalNavigation.Android
```

## Target framework and dependencies

- Target framework: `net10.0-android`
- `Sanet.MVVM.Core`
- `Microsoft.Extensions.DependencyInjection.Abstractions`

## Minimal usage

```csharp
using Sanet.MVVM.ExternalNavigation.Android.Extensions;

services.AddAndroidExternalNavigation();
```

The call registers a singleton `IExternalNavigationService` implementation.

```csharp
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.Core.ViewModels;

public sealed class AboutViewModel(IExternalNavigationService externalNavigation) : BaseViewModel
{
    public Task OpenRepositoryAsync() =>
        externalNavigation.OpenUrlAsync("https://github.com/anton-makarevich/Sanet.MVVM");

    public Task ContactAsync() =>
        externalNavigation.OpenEmailAsync("someone@example.com", "Hello");
}
```

## Platform notes

- `OpenUrlAsync` starts `Intent.ActionView` for the parsed URI and adds `ActivityFlags.NewTask`.
- `OpenEmailAsync` builds a `mailto:` URI and starts `Intent.ActionSendto`.
- The implementation catches and logs exceptions with `Console.WriteLine`.
- The returned task completes immediately. Task completion does not confirm that Android found a
  handler or opened an application.

## Notes and limitations

- The Android workload is required to build this package.
- The package provides the service only. The application is responsible for registering its own view
  models, navigation, and bootstrap services.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
