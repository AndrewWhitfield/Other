namespace Restart.Intune.UI.Avalonia
{
    using System;
    using Restart.Intune.UI.Avalonia.Windows;

    /// <summary>
    /// Factory that creates and returns the correct RestartRequiredWindow implementation
    /// based on the RESTARTINTUNE_UI environment variable.
    ///
    /// Routing rules:
    ///   RESTARTINTUNE_UI=avalonia  → Avalonia RestartRequiredWindow  (explicit opt-in)
    ///   (anything else / absent)   → default path (Avalonia in this workspace;
    ///                                WPF when the WPF project is wired in)
    ///
    /// Phase 2 constraint: default MUST remain the non-Avalonia path once WPF is wired.
    /// Avalonia is only selected via the explicit flag.
    /// </summary>
    public static class WindowLauncher
    {
        private const string EnvVar = "RESTARTINTUNE_UI";
        private const string AvaloniaFlag = "avalonia";

        /// <summary>
        /// Returns true when the Avalonia implementation has been explicitly requested.
        /// </summary>
        public static bool IsAvaloniaRequested()
        {
            var value = Environment.GetEnvironmentVariable(EnvVar);
            return string.Equals(value, AvaloniaFlag, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates and returns the Avalonia RestartRequiredWindow.
        /// Call only when <see cref="IsAvaloniaRequested"/> returns true (or in tests).
        /// </summary>
        public static RestartRequiredWindow LaunchAvaloniaRestartRequiredWindow()
            => new RestartRequiredWindow();

        // Stub for when WPF project is wired:
        // public static System.Windows.Window LaunchWpfRestartRequiredWindow()
        //     => new Restart.Intune.UI.Wpf.Windows.RestartRequiredWindow();
    }
}
