using Avalonia;
using Restart.Intune.UI.Avalonia;

namespace Restart.Intune.AppHost;

internal static class Program
{
    // Entry point. DI registration and platform wiring will be added in a later phase.
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
                  .UsePlatformDetect()
                  .WithInterFont()
                  .LogToTrace();
}
