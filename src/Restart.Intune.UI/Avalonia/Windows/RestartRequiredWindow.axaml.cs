namespace Restart.Intune.UI.Avalonia.Windows
{
    using System;
    using global::Avalonia;
    using global::Avalonia.Controls;
    using global::Avalonia.Interactivity;
    using Restart.Intune.UI.Configuration;
    using Restart.Intune.UI.Helpers;
    using Restart.Intune.UI.ViewModels;

    // ─────────────────────────────────────────────────────────────────────────
    // MAPPING NOTES
    // ─────────────────────────────────────────────────────────────────────────
    // [B10] WPF: DefaultDialogService shows ConfirmRestartDialog as a modal
    //       window and reads DialogResult (bool).
    //       Avalonia: replaced with await ShowDialog<bool?>() pattern.
    //       ConfirmRestartDialog calls this.Close(true/false); result is
    //       awaited here before executing restart.
    //
    // [B3]  FooterLogoSource — WPF code sets BitmapImage (System.Windows.Media).
    //       The ViewModel property must be updated to IImage for the binding to
    //       resolve. Direct code-behind assignment is retained as a workaround
    //       until the ViewModel is migrated.
    // ─────────────────────────────────────────────────────────────────────────

    public partial class RestartRequiredWindow : Window
    {
        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public RestartRequiredWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            ApplyFooterConfig();
            this.Opened += Window_Opened;
        }

        private void Window_Opened(object? sender, EventArgs e)
        {
            PositionBottomRight();
            this.SizeChanged += (_, _) => PositionBottomRight();
        }

        private void PositionBottomRight()
            => this.Position = MonitorHelper.GetBottomRightPosition(this, fallbackWidth: 530, fallbackHeight: 480);

        private void ApplyFooterConfig()
        {
            var settings = WindowConfigLoader.Load(nameof(RestartRequiredWindow));
            if (settings == null) return;

            if (!string.IsNullOrEmpty(settings.FooterText))
                ViewModel.FooterText = settings.FooterText;

            // [B3] Avalonia Bitmap replaces WPF BitmapImage.
            // ViewModel.FooterLogoSource must be updated to IImage for binding to work.
            // Uncomment after ViewModel is migrated:
            // if (!string.IsNullOrEmpty(settings.FooterImagePath))
            // {
            //     var uri = new Uri(settings.FooterImagePath, UriKind.Relative);
            //     ViewModel.FooterLogoSource = new Avalonia.Media.Imaging.Bitmap(
            //         Avalonia.Platform.AssetLoader.Open(uri));
            // }
        }

        private void BtnLater_Click(object sender, RoutedEventArgs e)
            => this.Close();

        private async void BtnRestartNow_Click(object sender, RoutedEventArgs e)
        {
            // [B10] Replaces WPF ConfirmRestartGate (DefaultDialogService + DialogResult).
            var dialog = new ConfirmRestartDialog();
            var result = await dialog.ShowDialog<bool?>(this);
            if (result == true)
                PerformRestart();
        }

        private static void PerformRestart()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("[DEBUG] Restart would be initiated now.");
#else
            SystemOperations.RestartMachine();
#endif
        }
    }
}
