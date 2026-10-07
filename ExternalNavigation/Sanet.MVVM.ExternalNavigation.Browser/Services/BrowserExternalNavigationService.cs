using System.Runtime.InteropServices.JavaScript;
using Sanet.MVVM.Core.Services;

namespace Sanet.MVVM.ExternalNavigation.Browser.Services;

public partial class BrowserExternalNavigationService : IExternalNavigationService
{
    public Task OpenUrlAsync(string url)
    {
        try
        {
            OpenUrlInNewTab(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open URL {url}: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    public Task OpenEmailAsync(string emailAddress, string subject)
    {
        try
        {
            var mailtoUri = $"mailto:{emailAddress}?subject={Uri.EscapeDataString(subject)}";
            OpenUrlInNewTab(mailtoUri);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open email client for {emailAddress}: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    [JSImport("globalThis.window.open")]
    private static partial void OpenUrlInNewTab(string url);
}
