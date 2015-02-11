using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace GameOfTasks
{
    public static class LocalizeHelper
    {
        private static ResourceLoader _resourceLoader;

        static LocalizeHelper()
        {
            _resourceLoader = new ResourceLoader();
        }

        public static string GetString(LocalizationKey key)
        {
            return _resourceLoader.GetString(key.ToString());
        }
    }
    public enum LocalizationKey
    {
        TaskNameLengthError,
        NotificationTaskStarting,
        NotificationEarlyStarting,
        NotificationEarlyDescription,
        DueDateMonthDayFormat
    }
}
