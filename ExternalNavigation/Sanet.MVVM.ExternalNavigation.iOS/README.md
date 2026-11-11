# Sanet.MVVM.ExternalNavigation.iOS

iOS implementation of `IExternalNavigationService` that opens URLs and email clients with UIKit.

## Installation

```bash
dotnet add package Sanet.MVVM.ExternalNavigation.iOS
```

## Target framework and dependencies

- Target framework: `net10.0-ios`
- `Sanet.MVVM.Core`
- `Microsoft.Extensions.DependencyInjection.Abstractions`

## Minimal usage

```csharp
using Sanet.MVVM.ExternalNavigation.iOS.Extensions;

services.AddIosExternalNavigation();
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

- `OpenUrlAsync` creates an `NSUrl` and calls
  `UIApplication.SharedApplication.OpenUrlAsync(url, new UIApplicationOpenUrlOptions())`.
- `OpenEmailAsync` builds a `mailto:` URI and calls the same UIKit API.
- The implementation catches and logs exceptions with `Console.WriteLine`.
- Task completion does not confirm that iOS found a handler or opened an application.

## Notes and limitations

- The `ios` workload is required, and this project must be built and packed on macOS.
- The package provides the service only. The application is responsible for registering its own view
  models, navigation, and bootstrap services.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
