using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WatchTogether.Chatting.Messages.Converters
{
    [ValueConversion(typeof(bool), typeof(HorizontalAlignment))]
    class BoolToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMessageMine = (bool)value;
            return (HorizontalAlignment)(isMessageMine ? 2 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var horizontalAlignment = (HorizontalAlignment)value;
            return System.Convert.ToBoolean((int)horizontalAlignment / 2);
        }
    }
}
