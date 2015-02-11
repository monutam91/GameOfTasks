using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GameOfTasks.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = value is bool && (bool)value;
            if (val)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = value is Visibility ? (Visibility)value : Visibility.Visible;
            if (val == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}
