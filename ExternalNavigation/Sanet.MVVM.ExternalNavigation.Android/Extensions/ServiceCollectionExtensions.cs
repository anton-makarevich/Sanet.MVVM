using Microsoft.Extensions.DependencyInjection;
using Sanet.MVVM.Core.Services;
using Sanet.MVVM.ExternalNavigation.Android.Services;

namespace Sanet.MVVM.ExternalNavigation.Android.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAndroidExternalNavigation(this IServiceCollection services)
    {
        services.AddSingleton<IExternalNavigationService, AndroidExternalNavigationService>();
        return services;
    }
}
