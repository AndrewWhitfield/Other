namespace Restart.Intune.UI.Helpers
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Cross-platform system-level operations.
    /// On Windows issues a graceful forced restart (shutdown /r /f /t 0).
    /// On other platforms issues the POSIX shutdown command.
    /// </summary>
    public static class SystemOperations
    {
        public static void RestartMachine()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            else
            {
                Process.Start(new ProcessStartInfo("shutdown", "-r now")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
        }
    }
}
