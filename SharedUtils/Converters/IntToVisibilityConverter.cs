using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GameOfTasks.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (int)value;
            if (args >= 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
