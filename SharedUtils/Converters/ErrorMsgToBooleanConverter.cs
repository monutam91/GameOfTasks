using System;
using Windows.UI.Xaml.Data;

namespace GameOfTasks.Converters
{
    public class ErrorMsgToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = value as string;
            if (!string.IsNullOrEmpty(val))
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
