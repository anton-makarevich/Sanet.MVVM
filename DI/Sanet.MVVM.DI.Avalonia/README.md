# Sanet.MVVM.DI.Avalonia

Avalonia `AppBuilder` extension that stores a Microsoft dependency-injection `IServiceCollection` in
application resources.

## Installation

```bash
dotnet add package Sanet.MVVM.DI.Avalonia
```

## Minimal usage

```csharp
using Avalonia;
using Sanet.MVVM.DI.Avalonia.Extensions;

public static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseDependencyInjection(services =>
        {
            services.AddSingleton<MainViewModel>();
        });
```

## Behavior

- `UseDependencyInjection(Action<IServiceCollection> registerServicesAction)` runs during
  `AppBuilder.AfterSetup`.
- It creates a new `ServiceCollection`, invokes the callback, and stores the collection in the
  application resources under `AppBuilderExtensions.ServiceCollectionResourceKey`
  (`"Sanet.MVVM.DI.Avalonia.ServiceCollectionResourceKey"`).
- In Avalonia design mode (`Design.IsDesignMode`) the method returns immediately and registers
  nothing.
- The method does not build an `IServiceProvider`.
- The method does not register navigation services or view mappings.

## Notes and limitations

- The application is responsible for reading the stored `IServiceCollection` from
  `Application.Current.Resources`, building the `IServiceProvider`, and passing that provider to the
  navigation service.
- Because the collection is stored as a resource, code that needs it must run after `AfterSetup`.

## Links

- [Repository README](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/README.md)

## License

MIT. See [LICENSE](https://github.com/anton-makarevich/Sanet.MVVM/blob/main/LICENSE).
