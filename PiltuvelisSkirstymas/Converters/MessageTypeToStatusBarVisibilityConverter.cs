using PiltuvelisSkirstymas.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PiltuvelisSkirstymas.Converters
{
    public class MessageTypeToStatusBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageType message && message != MessageType.Empty)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
