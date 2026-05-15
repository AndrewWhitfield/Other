namespace Restart.Intune.UI.Helpers
{
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using Microsoft.Win32;

    /// <summary>
    /// Writes Intune-managed postpone values to the Windows registry.
    /// On non-Windows platforms the write is a no-op — the registry does not exist.
    /// </summary>
    public static class RegistryHelper
    {
        private const string KeyPath   = @"SOFTWARE\RestartIntune";
        private const string ValueName = "PostponeTime";

        /// <summary>
        /// Persists <paramref name="postponeTime"/> (e.g. "2:30 PM") to
        /// HKCU\SOFTWARE\RestartIntune\PostponeTime.
        /// On non-Windows hosts this is a no-op.
        /// </summary>
        public static void WritePostponeValue(string postponeTime)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Debug.WriteLine($"[RegistryHelper] Non-Windows host — skipping registry write (value: {postponeTime}).");
                return;
            }

            using var key = Registry.CurrentUser.CreateSubKey(KeyPath, writable: true);
            key?.SetValue(ValueName, postponeTime, RegistryValueKind.String);
        }
    }
}
