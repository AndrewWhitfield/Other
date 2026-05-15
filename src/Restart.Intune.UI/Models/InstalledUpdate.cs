namespace Restart.Intune.UI.Models
{
    /// <summary>
    /// Represents a single installed Windows update shown in the
    /// "About this update" tab of the notification windows.
    /// </summary>
    public class InstalledUpdate
    {
        public string Id    { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
