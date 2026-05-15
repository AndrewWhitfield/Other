namespace Restart.Intune.UI.Avalonia.Windows
{
    using global::Avalonia.Controls;
    using global::Avalonia.Input;
    using global::Avalonia.Interactivity;

    // ─────────────────────────────────────────────────────────────────────────
    // MAPPING NOTES
    // ─────────────────────────────────────────────────────────────────────────
    // [B9]  WPF IsCancel="True" has no Avalonia equivalent.
    //       Escape key is handled explicitly in OnKeyDown.
    //
    // [B10] WPF DialogResult = true/false → this.Close(result).
    //       Calling pattern (replaces WPF ShowDialog() + DialogResult check):
    //
    //         var dialog = new ConfirmRestartDialog();
    //         var result = await dialog.ShowDialog<bool?>(ownerWindow);
    //         if (result == true) { /* restart */ }
    // ─────────────────────────────────────────────────────────────────────────

    public partial class ConfirmRestartDialog : Window
    {
        public ConfirmRestartDialog()
        {
            InitializeComponent();
            this.Opened += (_, _) =>
            {
                // Default focus on Cancel for safety — prevents accidental restart via Enter.
                // Matches WPF: BtnCancelDialog.Focus() in OnLoaded.
                BtnCancelDialog.Focus();
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            // [B9] Replaces WPF IsCancel="True" — Escape closes the dialog as cancelled.
            if (e.Key == Key.Escape)
            {
                this.Close(false); // [B10]
                e.Handled = true;
            }
        }

        private void BtnConfirmRestart_Click(object sender, RoutedEventArgs e)
        {
            // Disable immediately to prevent double-click / duplicate invocation.
            // Matches WPF: BtnConfirmRestart.IsEnabled = false; DialogResult = true;
            BtnConfirmRestart.IsEnabled = false;
            this.Close(true); // [B10]
        }

        private void BtnCancelDialog_Click(object sender, RoutedEventArgs e)
        {
            // Matches WPF: DialogResult = false;
            this.Close(false); // [B10]
        }
    }
}
