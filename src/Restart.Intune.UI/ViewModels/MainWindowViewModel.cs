namespace Restart.Intune.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Restart.Intune.UI.Models;

    /// <summary>
    /// State enum that drives the action-footer visibility in the notification windows.
    /// Default      → Postpone + Restart buttons
    /// PostponeMode → postpone time-picker row
    /// RestartMode  → Restart Now + Cancel row
    /// </summary>
    public enum ControlState { Default, PostponeMode, RestartMode }

    /// <summary>
    /// Shared ViewModel for all notification and restart windows.
    ///
    /// Visibility-state properties return bool (Avalonia IsVisible). [B1]
    /// The VisibilityToBoolConverter in AXAML is now a passthrough.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // ── INotifyPropertyChanged ───────────────────────────────────────────

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ── Title / branding ─────────────────────────────────────────────────

        private string _titleText = "Restart Required";
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; OnPropertyChanged(); }
        }

        private string _footerText = "IT Department";
        public string FooterText
        {
            get => _footerText;
            set { _footerText = value; OnPropertyChanged(); }
        }

        // FooterLogoSource: kept as object so the Avalonia Bitmap can be assigned
        // without a cast. [B3] Binding resolves only once IImage is used.
        private object? _footerLogoSource;
        public object? FooterLogoSource
        {
            get => _footerLogoSource;
            set { _footerLogoSource = value; OnPropertyChanged(); }
        }

        // ── Content text ─────────────────────────────────────────────────────

        private string _instructionText =
            "Your device is scheduled for a mandatory restart. " +
            "Please save your work and restart as soon as possible.";
        public string InstructionText
        {
            get => _instructionText;
            set { _instructionText = value; OnPropertyChanged(); }
        }

        private string _helpUrl = string.Empty;
        public string HelpUrl
        {
            get => _helpUrl;
            set { _helpUrl = value; OnPropertyChanged(); }
        }

        // ── Deadline / countdown ──────────────────────────────────────────────

        /// <summary>
        /// Formatted deadline string shown in the window header.
        /// Example: "Deadline: Thursday, May 7, 2026 at 05:00 PM"
        /// Populated by the window or a service once the deadline is known.
        /// </summary>
        private string _deadlineText = "Deadline: Thursday, May 7, 2026 at 05:00 PM";
        public string DeadlineText
        {
            get => _deadlineText;
            set { _deadlineText = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Formatted time-remaining string shown in the alert banner.
        /// Example: "Time remaining: 2d 4h 49m"
        /// Updated by a DispatcherTimer in the window code-behind.
        /// </summary>
        private string _timeRemainingText = "Time remaining: 2d 4h 49m";
        public string TimeRemainingText
        {
            get => _timeRemainingText;
            set { _timeRemainingText = value; OnPropertyChanged(); }
        }

        // ── Build version ─────────────────────────────────────────────────────

        private string _buildVersion = "v2.0.0";
        public string BuildVersion
        {
            get => _buildVersion;
            set { _buildVersion = value; OnPropertyChanged(); }
        }

        // ── Installed updates (About this update tab) ─────────────────────────

        public ObservableCollection<InstalledUpdate> InstalledUpdates { get; } = new();

        // ── Toggle strip ─────────────────────────────────────────────────────

        private bool _isShowingUpdateDetails;
        public bool IsShowingUpdateDetails
        {
            get => _isShowingUpdateDetails;
            set
            {
                _isShowingUpdateDetails = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScheduledRestartVisibility));
                OnPropertyChanged(nameof(UpdateDetailsVisibility));
                OnPropertyChanged(nameof(ToggleStripVisibility));
            }
        }

        // ── Visibility properties (bool for Avalonia IsVisible) [B1] ─────────

        public bool ToggleStripVisibility
            => InstalledUpdates.Count > 0;

        public bool ScheduledRestartVisibility
            => !_isShowingUpdateDetails;

        public bool UpdateDetailsVisibility
            => _isShowingUpdateDetails;

        // ── Control state (action footer state machine) ───────────────────────

        private ControlState _controlState = ControlState.Default;
        public ControlState ControlState
        {
            get => _controlState;
            set
            {
                _controlState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DefaultActionsVisibility));
                OnPropertyChanged(nameof(PostponeModeActionsVisibility));
                OnPropertyChanged(nameof(RestartModeActionsVisibility));
            }
        }

        public bool DefaultActionsVisibility
            => _controlState == ControlState.Default;

        public bool PostponeModeActionsVisibility
            => _controlState == ControlState.PostponeMode;

        public bool RestartModeActionsVisibility
            => _controlState == ControlState.RestartMode;
    }
}
