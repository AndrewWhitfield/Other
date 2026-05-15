namespace Restart.Intune.UI.Avalonia.Converters
{
    using System;
    using System.Globalization;
    using global::Avalonia.Data.Converters;

    /// <summary>
    /// Avalonia equivalent of Restart.Intune.UI.Converters.StringNotEmptyToVisibilityConverter.
    ///
    /// WPF equivalent returned System.Windows.Visibility (Visible / Collapsed).
    /// Avalonia uses bool for IsVisible, so this converter returns bool directly.
    ///
    /// Returns true when the string value is non-null and non-empty; false otherwise.
    /// </summary>
    public class StringNotEmptyToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !string.IsNullOrEmpty(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
