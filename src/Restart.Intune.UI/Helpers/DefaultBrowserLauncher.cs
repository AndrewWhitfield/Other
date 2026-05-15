namespace Restart.Intune.UI.Helpers
{
    using System.Diagnostics;

    /// <summary>
    /// Production implementation of <see cref="IBrowserLauncher"/>.
    /// Uses the OS default handler for http/https URIs.
    /// </summary>
    public class DefaultBrowserLauncher : IBrowserLauncher
    {
        public void Open(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
