using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace GameOfTasks.Converters
{
    public class DateTimeToDueDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var now = DateTime.Now;
            var val = value is DateTime ? (DateTime) value : now;
            if (now.DayOfYear == val.DayOfYear)
            {
                return val.ToString("HH:mm");
            }
            if (GetIso8601WeekOfYear(val) == GetIso8601WeekOfYear(now))
            {
                return val.ToString("dddd, HH:mm");
            }
            if (val.Year == DateTime.Now.Year)
            {
                return val.ToString(LocalizeHelper.GetString(LocalizationKey.DueDateMonthDayFormat));
            }
            return val.ToString("yyyy MMMM dd, HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
