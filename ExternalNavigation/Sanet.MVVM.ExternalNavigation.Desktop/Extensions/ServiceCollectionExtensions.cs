using Microsoft.Extensions.DependencyInjection;
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.ExternalNavigation.Desktop.Services;

namespace Sanet.MVVM.ExternalNavigation.Desktop.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDesktopExternalNavigation(this IServiceCollection services)
    {
        services.AddSingleton<IExternalNavigationService, DesktopExternalNavigationService>();
        return services;
    }
}
