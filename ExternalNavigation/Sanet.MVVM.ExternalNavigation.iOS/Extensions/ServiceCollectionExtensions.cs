using Microsoft.Extensions.DependencyInjection;
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.ExternalNavigation.iOS.Services;

namespace Sanet.MVVM.ExternalNavigation.iOS.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIosExternalNavigation(this IServiceCollection services)
    {
        services.AddSingleton<IExternalNavigationService, IosExternalNavigationService>();
        return services;
    }
}
