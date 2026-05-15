namespace Restart.Intune.UI.Avalonia.Converters
{
    using System;
    using System.Globalization;
    using global::Avalonia.Data.Converters;

    /// <summary>
    /// Avalonia equivalent of Restart.Intune.UI.Converters.InverseBooleanConverter.
    /// Maps bool → !bool in both directions.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;
    }
}
