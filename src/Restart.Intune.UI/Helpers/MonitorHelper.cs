namespace Restart.Intune.UI.Helpers
{
    using global::Avalonia;
    using global::Avalonia.Controls;

    /// <summary>
    /// Provides the primary monitor's work area rectangle (screen minus taskbar).
    /// Returns a safe fallback when Avalonia screen information is unavailable
    /// (e.g., during headless tests).
    /// </summary>
    public static class MonitorHelper
    {
        private const int EdgeMargin = 20; // physical pixels of padding from screen edges

        /// <summary>
        /// Returns the work area of the primary screen as a <see cref="Rect"/>.
        /// Coordinates are in physical pixels, matching <see cref="PixelPoint"/>.
        /// </summary>
        public static Rect GetPrimaryMonitorWorkArea()
        {
            try
            {
                var screens = (global::Avalonia.Application.Current?.ApplicationLifetime as
                    global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)
                    ?.MainWindow?.Screens?.Primary;

                if (screens != null)
                {
                    var wa = screens.WorkingArea;
                    return new Rect(wa.X, wa.Y, wa.Width, wa.Height);
                }
            }
            catch
            {
                // Fallback if Avalonia is not fully initialised (tests, etc.)
            }

            return new Rect(0, 0, 1920, 1040);
        }

        /// <summary>
        /// Calculates the bottom-right <see cref="PixelPoint"/> so that
        /// <paramref name="window"/> sits fully on-screen, above the taskbar and
        /// clear of the right edge, respecting the display's DPI scaling.
        /// </summary>
        /// <param name="window">The window to position.</param>
        /// <param name="fallbackWidth">Design-time logical width used before layout runs.</param>
        /// <param name="fallbackHeight">Design-time logical height used before layout runs.</param>
        public static PixelPoint GetBottomRightPosition(Window window, double fallbackWidth, double fallbackHeight)
        {
            var workingArea = GetPrimaryMonitorWorkArea();

            // Bounds are in logical DIPs — scale to physical pixels using the
            // window's actual DPI factor so the result aligns with WorkingArea.
            var scale = window.RenderScaling > 0 ? window.RenderScaling : 1.0;
            var physW = (window.Bounds.Width  > 0 ? window.Bounds.Width  : fallbackWidth)  * scale;
            var physH = (window.Bounds.Height > 0 ? window.Bounds.Height : fallbackHeight) * scale;

            return new PixelPoint(
                (int)(workingArea.Right  - physW - EdgeMargin),
                (int)(workingArea.Bottom - physH - EdgeMargin));
        }
    }
}
