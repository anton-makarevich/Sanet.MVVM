# Sanet.MVVM.ExternalNavigation.Desktop

Desktop implementation of `IExternalNavigationService` that opens URLs and email clients with
OS-specific mechanisms.

## Installation

```bash
dotnet add package Sanet.MVVM.ExternalNavigation.Desktop
```

## Minimal usage

```csharp
using Sanet.MVVM.ExternalNavigation.Desktop.Extensions;

services.AddDesktopExternalNavigation();
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

- On Windows, the service uses `Process.Start` with `UseShellExecute = true` for both URLs and
  `mailto:` URIs.
- On Linux, it runs `xdg-open`.
- On macOS, it runs `open`.
- On any other OS platform, the service does nothing.
- The implementation catches and logs exceptions with `Console.WriteLine`.
- The returned task completes immediately. Task completion does not confirm that the shell or an
  application handled the request.

## Notes and limitations

- The package targets `net10.0` and has no Avalonia dependency.
- The package provides the service only. The application is responsible for registering its own view
  models, navigation, and bootstrap services.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
