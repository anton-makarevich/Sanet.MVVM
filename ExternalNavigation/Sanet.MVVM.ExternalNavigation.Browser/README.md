# Sanet.MVVM.ExternalNavigation.Browser

Browser (WebAssembly) implementation of `IExternalNavigationService` that opens URLs through
JavaScript `window.open`.

## Installation

```bash
dotnet add package Sanet.MVVM.ExternalNavigation.Browser
```

## Target framework and dependencies

- Target framework: `net10.0-browser`
- `Sanet.MVVM.Core`
- `Microsoft.Extensions.DependencyInjection.Abstractions`

## Minimal usage

```csharp
using Sanet.MVVM.ExternalNavigation.Browser.Extensions;

services.AddBrowserExternalNavigation();
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

- `OpenUrlAsync` calls `window.open` through the `[JSImport("globalThis.window.open")]` source
  generator.
- `OpenEmailAsync` builds a `mailto:` URI and passes it to the same `window.open` call.
- Browser popup blockers can stop the new window or tab.
- The implementation catches and logs exceptions with `Console.WriteLine`.
- The returned task completes immediately. Task completion does not confirm that the browser opened a
  window or found an email handler.

## Notes and limitations

- The `wasm-tools` workload is required to build this package.
- The package provides the service only. The application is responsible for registering its own view
  models, navigation, and bootstrap services.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
