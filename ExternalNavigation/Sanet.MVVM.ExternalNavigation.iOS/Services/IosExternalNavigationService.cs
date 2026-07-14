using Foundation;
using Sanet.MVVM.Core.Services;
using UIKit;

namespace Sanet.MVVM.ExternalNavigation.iOS.Services;

public class IosExternalNavigationService : IExternalNavigationService
{
    public async Task OpenUrlAsync(string url)
    {
        try
        {
            var nsUrl = new NSUrl(url);
            await UIApplication.SharedApplication.OpenUrlAsync(nsUrl, new UIApplicationOpenUrlOptions());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open URL {url}: {ex.Message}");
        }
    }

    public async Task OpenEmailAsync(string emailAddress, string subject)
    {
        try
        {
            var mailtoUri = $"mailto:{emailAddress}?subject={Uri.EscapeDataString(subject)}";
            var nsUrl = new NSUrl(mailtoUri);
            await UIApplication.SharedApplication.OpenUrlAsync(nsUrl, new UIApplicationOpenUrlOptions());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open email client for {emailAddress}: {ex.Message}");
        }
    }
}
