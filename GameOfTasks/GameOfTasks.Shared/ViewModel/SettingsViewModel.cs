using System;
using System.Collections.Generic;
using GameOfTasks.Common;

namespace GameOfTasks.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public bool EarlyNotificationEnabled
        {
            get { return IsolatedSettingsHelper.GetSettingsValue<bool>(Settings.EnableEarlyNotification); }
            set
            {
                if (value)
                {
                    var isActive = PaymentHelper.GetLicenceInfo(LicenseKeys.EarlyNotificationLicence).IsActive;
                    if (isActive)
                    {
                        IsolatedSettingsHelper.SetSettingsValue(Settings.EnableEarlyNotification, true);
                        RaisePropertyChanged();
                    }
                    else
                    {
                        var waiter = PaymentHelper.PurchaseLicence(LicenseKeys.EarlyNotificationLicence).GetAwaiter();
                        waiter.OnCompleted(delegate
                        {
                            IsolatedSettingsHelper.SetSettingsValue(Settings.EnableEarlyNotification, waiter.GetResult());
                            RaisePropertyChanged();
                        });
                    }
                }
                else
                {
                    IsolatedSettingsHelper.SetSettingsValue(Settings.EnableEarlyNotification, false);
                }
            }
        }


        public TimeSpan EarlyNotificationTime
        {
            get
            {
                return IsolatedSettingsHelper.GetSettingsValue<TimeSpan>(Settings.EarlyNotification);
            }
            set
            {
                IsolatedSettingsHelper.SetSettingsValue(Settings.EarlyNotification, value);
                RaisePropertyChanged();
            }
        }

        public List<string> FlyoutItem
        {
            get
            {
                var list = new List<string>();
                for (var i = 0.5; i < 24; i += 0.5)
                {
                    var wholePart = (int)Math.Truncate(i);
                    var minutes = (int)((i - wholePart) * 60);
                    list.Add(new TimeSpan(wholePart, minutes, 0).ToString("hh':'mm"));
                }
                return list;
            }
        }

        public SettingsViewModel()
        {
        }
    }
}
