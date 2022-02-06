using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WatchTogether.Chatting.Messages.Converters
{
    [ValueConversion(typeof(bool), typeof(FlowDirection))]
    class BoolToFlowDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMessageMine = (bool)value;
            return (FlowDirection)(isMessageMine ? 1 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flowDirection = (FlowDirection)value;
            return System.Convert.ToBoolean((int)flowDirection);
        }
    }
}
