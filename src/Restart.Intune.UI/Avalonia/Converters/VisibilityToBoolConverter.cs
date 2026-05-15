namespace Restart.Intune.UI.Avalonia.Converters
{
    using System;
    using System.Globalization;
    using global::Avalonia.Data.Converters;

    /// <summary>
    /// Bridge converter: accepts a bool from the ViewModel and returns it unchanged
    /// so that AXAML bindings using this converter continue to work without modification.
    ///
    /// [B1] Originally designed to convert System.Windows.Visibility → bool.
    /// The ViewModel now returns bool directly (Avalonia-compatible), so this
    /// converter is a passthrough. It is retained so existing AXAML binding
    /// expressions require no changes.
    /// </summary>
    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
