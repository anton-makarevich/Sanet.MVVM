using System.Diagnostics;
using System.Runtime.InteropServices;
using Sanet.MVVM.Core.Services;

namespace Sanet.MVVM.ExternalNavigation.Desktop.Services;

public class DesktopExternalNavigationService : IExternalNavigationService
{
    public Task OpenUrlAsync(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = mailtoUri,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", mailtoUri);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", mailtoUri);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open email client for {emailAddress}: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
