using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using GameOfTasks;
using GameOfTasks.Model;

namespace PhoneBackgroundAgent
{
    public sealed class NotificationBackgroundAgent : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var date = DateTime.Now.AddMinutes(30);
            var db = new DbContext();
            var jobsNow =
                db.ToDoJobs.Where(
                    job =>
                        !job.NotificationScheduled && job.IsHavingDueDate && job.NotificationTime.CompareTo(date) <= 0 &&
                        job.NotificationTime.CompareTo(DateTime.Now) > 0).ToList();
            foreach (var toDoJob in jobsNow)
            {
                var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                var toastElement = notificationXml.GetElementsByTagName("text");
                toastElement[0].AppendChild(
                    notificationXml.CreateTextNode(
                        String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationTaskStarting),
                            toDoJob.TaskName)));
                var toastNotification = new ScheduledToastNotification(notificationXml, toDoJob.NotificationTime);
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
                toDoJob.NotificationScheduled = true;
            }
            db.Connection.UpdateAll(jobsNow);
            if (IsolatedSettingsHelper.GetSettingsValue<bool>(Settings.EnableEarlyNotification))
            {
                var earlyNotification = IsolatedSettingsHelper.GetSettingsValue<TimeSpan>(Settings.EarlyNotification);
                var earlyTime = earlyNotification.TotalHours;
                var earlyJobs =
                    db.ToDoJobs.Where(
                        job =>
                            !job.EarlyNotificationScheduled && job.IsHavingDueDate &&
                            job.NotificationTime.AddHours(-earlyTime).CompareTo(date) <= 0 &&
                            job.NotificationTime.AddHours(-earlyTime).CompareTo(DateTime.Now) > 0).ToList();
                foreach (var earlyJob in earlyJobs)
                {
                    var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    var toastElement = notificationXml.GetElementsByTagName("text");
                    toastElement[0].AppendChild(
                        notificationXml.CreateTextNode(
                            String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationEarlyStarting),
                                earlyJob.TaskName)));
                    toastElement[1].AppendChild(
                        notificationXml.CreateTextNode(
                            String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationEarlyDescription),
                                earlyJob.TaskName, earlyNotification.Hours)));
                    var toastNotification = new ScheduledToastNotification(notificationXml,
                        earlyJob.NotificationTime.AddHours(-earlyTime));
                    ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
                    earlyJob.EarlyNotificationScheduled = true;
                }
                db.Connection.UpdateAll(earlyJobs);

                var nextJobs = db.ToDoJobs.Where(job => job.IsHavingDueDate).Take(5).ToList();
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueue(true);
                updater.Clear();
                foreach (var job in nextJobs)
                {
                    XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText05);

                    var texts = tileXml.GetElementsByTagName(TextElement);
                    texts[0].InnerText = job.TaskName;
                    texts[1].InnerText = job.NotificationTime.ToString("dddd, HH:mm");

                    // Create a new tile notification. 
                    updater.Update(new TileNotification(tileXml));
                }
            }
        }
        internal static string TextElement = "text";
    }
}
