using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Sanet.MVVM.DI.Avalonia.Extensions;

public static class AppBuilderExtensions
{
    public static AppBuilder UseDependencyInjection(this AppBuilder appBuilder, Action<IServiceCollection> registerServicesAction)
    {
        if (Design.IsDesignMode) { return appBuilder; }
        
        appBuilder.AfterSetup(x =>
        {
            var services = new ServiceCollection();

            registerServicesAction(services);
            
            x.Instance?.Resources.Add(
                ServiceCollectionResourceKey,
                services);
        });

        return appBuilder;
    }
    public const string ServiceCollectionResourceKey = $"Sanet.MVVM.DI.Avalonia.{nameof(ServiceCollectionResourceKey)}";
}