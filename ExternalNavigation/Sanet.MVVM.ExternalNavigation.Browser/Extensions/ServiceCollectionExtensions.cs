using Microsoft.Extensions.DependencyInjection;
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.ExternalNavigation.Browser.Services;

namespace Sanet.MVVM.ExternalNavigation.Browser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBrowserExternalNavigation(this IServiceCollection services)
    {
        services.AddSingleton<IExternalNavigationService, BrowserExternalNavigationService>();
        return services;
    }
}
