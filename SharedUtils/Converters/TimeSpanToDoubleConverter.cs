using System;
using Windows.UI.Xaml.Data;

namespace GameOfTasks.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = value is TimeSpan ? (TimeSpan)value : TimeSpan.FromHours(1);
            return (double)val.Hours;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = value is double ? (double)value : 1.0;
            return TimeSpan.FromHours(val);
        }
    }
}
