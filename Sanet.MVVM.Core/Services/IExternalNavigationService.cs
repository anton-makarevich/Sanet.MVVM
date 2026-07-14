namespace Sanet.MVVM.Core.Services;

public interface IExternalNavigationService
{
    Task OpenUrlAsync(string url);
    Task OpenEmailAsync(string emailAddress, string subject);
}
