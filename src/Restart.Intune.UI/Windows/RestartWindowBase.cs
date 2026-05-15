namespace Restart.Intune.UI.Windows
{
    using System;

    /// <summary>
    /// Provides the shared business-logic methods for postpone time calculation
    /// and postpone-limit enforcement. These were previously on the WPF base class.
    ///
    /// All methods are static so they can be called from Avalonia code-behind
    /// without inheriting the WPF base class. [B4]
    /// </summary>
    public static class RestartWindowBase
    {
        /// <summary>The maximum number of postpones a user may take.</summary>
        public const int MaxPostpones = 5;

        // ── Postpone time bounds ──────────────────────────────────────────────

        /// <summary>
        /// Returns the earliest selectable postpone time (now + 30 minutes,
        /// rounded to the next whole half-hour).
        /// </summary>
        public static DateTime GetLowerBound(DateTime now)
        {
            var candidate = now.AddMinutes(30);
            // Round up to the next 30-minute mark.
            var minutes = candidate.Minute < 30 ? 30 : 0;
            var addHour  = candidate.Minute >= 30 ? 1 : 0;
            return new DateTime(candidate.Year, candidate.Month, candidate.Day,
                candidate.Hour + addHour, minutes, 0);
        }

        /// <summary>
        /// Returns the latest selectable postpone time (now + 4 hours,
        /// rounded down to the nearest whole half-hour).
        /// </summary>
        public static DateTime GetUpperBound(DateTime now)
        {
            var candidate = now.AddHours(4);
            var minutes   = candidate.Minute >= 30 ? 30 : 0;
            return new DateTime(candidate.Year, candidate.Month, candidate.Day,
                candidate.Hour, minutes, 0);
        }

        // ── Time increment / decrement ────────────────────────────────────────

        /// <summary>
        /// Returns the next postpone time slot (30-minute steps), clamped to
        /// <paramref name="upper"/>. If <paramref name="current"/> is null the
        /// lower bound is returned instead.
        /// </summary>
        public static DateTime ComputeIncrementedPostponeTime(DateTime? current, DateTime upper)
        {
            if (current == null) return GetLowerBound(DateTime.Now);
            var next = current.Value.AddMinutes(30);
            return next > upper ? upper : next;
        }

        /// <summary>
        /// Returns the previous postpone time slot (30-minute steps), clamped to
        /// <paramref name="lower"/>. If <paramref name="current"/> is null the
        /// lower bound is returned instead.
        /// </summary>
        public static DateTime ComputeDecrementedPostponeTime(DateTime? current, DateTime lower)
        {
            if (current == null) return lower;
            var prev = current.Value.AddMinutes(-30);
            return prev < lower ? lower : prev;
        }

        // ── Postpone limit evaluation ─────────────────────────────────────────

        /// <summary>
        /// Returns warning / disable state based on the remaining postpone count.
        /// </summary>
        public static (bool showWarning, string warningMessage, bool disableButton)
            EvaluatePostponeAction(int postponesRemaining)
        {
            return postponesRemaining switch
            {
                1  => (true,
                       "⚠ You have 1 postpone remaining. " +
                       "The restart cannot be deferred further after this.",
                       false),
                <= 0 => (true,
                          "You have reached the maximum number of postpones. " +
                          "Please restart your device now.",
                          true),
                _  => (false, string.Empty, false)
            };
        }
    }
}
