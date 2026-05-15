namespace Restart.Intune.UI.Avalonia.Windows
{
    using System;
    using global::Avalonia;
    using global::Avalonia.Controls;
    using global::Avalonia.Interactivity;
    using Restart.Intune.UI.Helpers;
    using Restart.Intune.UI.ViewModels;

    // ─────────────────────────────────────────────────────────────────────────
    // MAPPING NOTES
    // ─────────────────────────────────────────────────────────────────────────
    // [B4]  RestartWindowBase (WPF) cannot be reused. All event handler logic
    //       previously defined on the base class is reproduced here and
    //       delegates to the same static / internal methods on RestartWindowBase
    //       for testable business logic (no duplication of rules).
    //
    // [B8]  WPF Window.Loaded → Avalonia Window.Opened.
    //       Opened fires once the window is visible on screen, which matches
    //       the WPF Loaded semantics used by PositionBottomRight().
    // ─────────────────────────────────────────────────────────────────────────

    public partial class CBANotificationWindow : Window
    {
        private readonly IBrowserLauncher _browserLauncher;

        private DateTime? _selectedPostponeTime;
        private int _postponesRemaining = 5; // MaxPostpones

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public CBANotificationWindow()
        {
            _browserLauncher = new DefaultBrowserLauncher();
            InitializeComponent();
            this.DataContext = ViewModel;

            // [B8] Opened replaces WPF Window.Loaded for initial positioning.
            this.Opened += Window_Opened;
        }

        // ── Window lifecycle ─────────────────────────────────────────────────

        private void Window_Opened(object sender, EventArgs e)
        {
            PositionBottomRight();
            this.SizeChanged += (_, _) => PositionBottomRight();
        }

        private void PositionBottomRight()
            => this.Position = MonitorHelper.GetBottomRightPosition(this, fallbackWidth: 460, fallbackHeight: 400);

        // ── Button event handlers ─────────────────────────────────────────────

        private void BtnEnterPostponeMode_Click(object sender, RoutedEventArgs e)
            => ViewModel.ControlState = ControlState.PostponeMode;

        private void BtnEnterRestartMode_Click(object sender, RoutedEventArgs e)
            => ViewModel.ControlState = ControlState.RestartMode;

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => ViewModel.ControlState = ControlState.Default;

        private void TimeIncrementButton_Click(object sender, RoutedEventArgs e)
        {
            var upper = global::Restart.Intune.UI.Windows.RestartWindowBase.GetUpperBound(DateTime.Now);
            var next  = global::Restart.Intune.UI.Windows.RestartWindowBase
                .ComputeIncrementedPostponeTime(_selectedPostponeTime, upper);
            if (next != _selectedPostponeTime) { _selectedPostponeTime = next; UpdatePostponeTimeDisplay(); }
        }

        private void TimeDecrementButton_Click(object sender, RoutedEventArgs e)
        {
            var lower = global::Restart.Intune.UI.Windows.RestartWindowBase.GetLowerBound(DateTime.Now);
            var prev  = global::Restart.Intune.UI.Windows.RestartWindowBase
                .ComputeDecrementedPostponeTime(_selectedPostponeTime, lower);
            if (prev != _selectedPostponeTime) { _selectedPostponeTime = prev; UpdatePostponeTimeDisplay(); }
        }

        private void BtnPostpone_Click(object sender, RoutedEventArgs e)
        {
            var postponeTime = _selectedPostponeTime?.ToString("h:mm tt") ?? "--:-- --";
            _postponesRemaining--;
            var state = global::Restart.Intune.UI.Windows.RestartWindowBase
                .EvaluatePostponeAction(_postponesRemaining);

            var warning = this.FindControl<TextBlock>("TxtPostponeWarning");
            if (warning != null && state.showWarning)
            {
                warning.Text      = state.warningMessage;
                warning.IsVisible = true;
            }

            if (state.disableButton)
            {
                var btn = this.FindControl<Button>("BtnPostpone");
                if (btn != null) btn.IsEnabled = false;
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Postpone set to {postponeTime}.");
#else
            RegistryHelper.WritePostponeValue(postponeTime);
#endif
            this.Close();
        }

        private void BtnRestartNow_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("[DEBUG] Restart would be initiated now.");
#else
            SystemOperations.RestartMachine();
#endif
        }

        private void HelpLink_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.HelpUrl))
                HyperlinkNavigationHandler.Handle(ViewModel.HelpUrl, _browserLauncher, "CBANotificationWindow");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void UpdatePostponeTimeDisplay()
        {
            var tb = this.FindControl<TextBlock>("PostponeTimeTextBlock");
            if (tb != null)
                tb.Text = _selectedPostponeTime.HasValue
                    ? _selectedPostponeTime.Value.ToString("h:mm tt")
                    : "--:-- --";
        }
    }
}
