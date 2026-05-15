namespace Restart.Intune.UI.Helpers
{
    /// <summary>
    /// Abstraction for launching a URL in the default browser.
    /// Allows the real browser launch to be replaced in tests.
    /// </summary>
    public interface IBrowserLauncher
    {
        void Open(string url);
    }
}
