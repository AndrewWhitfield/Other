using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Restart.Intune.UI.Avalonia;

// Bootstrapping only. Window creation and DI wiring are deferred to later phases.
public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Phase 2 routing: default path is Avalonia (sole UI in this workspace).
            // Once WPF is wired, non-Avalonia flag must route to WPF instead.
            // See WindowLauncher for flag details (RESTARTINTUNE_UI=avalonia).
            desktop.MainWindow = WindowLauncher.IsAvaloniaRequested()
                ? WindowLauncher.LaunchAvaloniaRestartRequiredWindow()
                : WindowLauncher.LaunchAvaloniaRestartRequiredWindow(); // default: same until WPF wired
        }

        base.OnFrameworkInitializationCompleted();
    }
}

