using Android.Content;
using Sanet.MVVM.Core.Services;

namespace Sanet.MVVM.ExternalNavigation.Android.Services;

public class AndroidExternalNavigationService : IExternalNavigationService
{
    public Task OpenUrlAsync(string url)
    {
        try
        {
            var uri = global::Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NewTask);

            Application.Context.StartActivity(intent);
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
            var uri = global::Android.Net.Uri.Parse(mailtoUri);
            var intent = new Intent(Intent.ActionSendto, uri);
            intent.AddFlags(ActivityFlags.NewTask);

            Application.Context.StartActivity(intent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open email client for {emailAddress}: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
