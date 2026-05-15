namespace Restart.Intune.UI.Configuration
{
    using System.Diagnostics;

    /// <summary>
    /// Loads per-window configuration from disk.
    /// Returns null when no config is found so callers can apply defaults.
    /// </summary>
    public static class WindowConfigLoader
    {
        public static WindowConfig? Load(string windowName)
        {
            // Stub: real implementation will read from a JSON file or registry key.
            Debug.WriteLine($"[WindowConfigLoader] No config found for '{windowName}'. Using defaults.");
            return null;
        }
    }
}
