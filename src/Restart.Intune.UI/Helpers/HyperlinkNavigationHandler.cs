namespace Restart.Intune.UI.Helpers
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Handles navigation when the user clicks a hyperlink in a notification window.
    /// Opens the URL via the supplied <see cref="IBrowserLauncher"/> and logs the
    /// source window name for diagnostics.
    /// </summary>
    public static class HyperlinkNavigationHandler
    {
        public static void Handle(string url, IBrowserLauncher launcher, string sourceWindow)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            Debug.WriteLine($"[{sourceWindow}] Opening help URL: {url}");
            try
            {
                launcher.Open(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{sourceWindow}] Failed to open URL '{url}': {ex.Message}");
            }
        }
    }
}
