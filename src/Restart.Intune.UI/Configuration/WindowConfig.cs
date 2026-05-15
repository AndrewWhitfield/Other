namespace Restart.Intune.UI.Configuration
{
    /// <summary>
    /// Per-window configuration loaded from external config (JSON / registry).
    /// Mirrors the WPF WindowConfig class.
    /// </summary>
    public class WindowConfig
    {
        public string? FooterText      { get; set; }
        public string? FooterImagePath { get; set; }
    }
}
