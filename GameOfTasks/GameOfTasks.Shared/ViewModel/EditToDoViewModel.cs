using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using GalaSoft.MvvmLight.Command;
using GameOfTasks.Common;
using GameOfTasks.Model;

namespace GameOfTasks.ViewModel
{
    public class EditToDoViewModel : ViewModelBase
    {
        private ToDoJob _editedTask;
        private string _taskNameError;
        private readonly RelayCommand _editTaskCommand;

        public bool IsHavingDueDate
        {
            get { return _editedTask.IsHavingDueDate; }
            set
            {
                _editedTask.IsHavingDueDate = value;
                RaisePropertyChanged();
            }
        }

        public TimeSpan NotifyTime
        {
            get { return new TimeSpan(_editedTask.NotificationTime.Hour, _editedTask.NotificationTime.Minute, _editedTask.NotificationTime.Second); }
            set
            {
                _editedTask.NotificationTime = _editedTask.NotificationTime.Date + value;
                RaisePropertyChanged();
            }
        }

        public DateTimeOffset NotifyDate
        {
            get { return _editedTask.NotificationTime; }
            set
            {
                var tmpTs = new TimeSpan(_editedTask.NotificationTime.Hour, _editedTask.NotificationTime.Minute, _editedTask.NotificationTime.Second);
                _editedTask.NotificationTime = value.DateTime;
                _editedTask.NotificationTime = _editedTask.NotificationTime.Date + tmpTs;
                RaisePropertyChanged();
            }
        }

        public DateTimeOffset MinYear
        {
            get
            {
                return new DateTimeOffset(DateTime.Now);
            }
        }

        public string TaskName
        {
            get
            {
                return _editedTask.TaskName;
            }
            set
            {
                _editedTask.TaskName = value;
                if (_editedTask.TaskName.Length < 3 || _editedTask.TaskName.Length > 100)
                {
                    TaskNameError = LocalizeHelper.GetString(LocalizationKey.TaskNameLengthError);
                }
                else
                {
                    TaskNameError = "";
                }
                RaisePropertyChanged();
            }
        }

        public string TaskNameError
        {
            get { return _taskNameError; }
            private set
            {
                _taskNameError = value;
                RaisePropertyChanged();
            }
        }

        public int Points
        {
            get
            {
                return _editedTask.HardnessPoints;
            }
            set
            {
                _editedTask.HardnessPoints = value;
                RaisePropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _editedTask.Description;
            }
            set
            {
                _editedTask.Description = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand EditTaskCommand
        {
            get { return _editTaskCommand; }
        }

        public EditToDoViewModel()
        {
            _editedTask = new ToDoJob();
            _editTaskCommand = new RelayCommand(EditTask);
        }

        private async void EditTask()
        {
            var db = new DbContextAsync();
            await db.Init();
            var job =
                await
                    db.ToDoJobs.Where(task => task.JobId == _editedTask.JobId).FirstAsync();
            if (job != null)
            {
                job = _editedTask;

                if (IsolatedSettingsHelper.GetSettingsValue<bool>(Settings.EnableEarlyNotification))
                {
                    var earlyNotify = IsolatedSettingsHelper.GetSettingsValue<TimeSpan>(Settings.EarlyNotification);
                    var earlyDate = job.NotificationTime - earlyNotify;
                    if (earlyDate.CompareTo(DateTime.Now) > 0 && earlyDate.CompareTo(DateTime.Now.AddMinutes(30)) <= 0)
                    {
                        var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                        var toastElement = notificationXml.GetElementsByTagName("text");
                        toastElement[0].AppendChild(
                            notificationXml.CreateTextNode(
                                String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationEarlyStarting),
                                    job.TaskName)));
                        toastElement[1].AppendChild(
                            notificationXml.CreateTextNode(
                                String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationEarlyDescription),
                                    job.TaskName, earlyNotify.Hours)));
                        var toastNotification = new ScheduledToastNotification(notificationXml, earlyDate);
                        ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
                    }
                    job.EarlyNotificationScheduled = true;
                }

                if (job.IsHavingDueDate && job.NotificationTime.CompareTo(DateTime.Now) > 0 &&
                    job.NotificationTime.CompareTo(DateTime.Now.AddMinutes(30)) <= 0)
                {
                    var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    var toastElement = notificationXml.GetElementsByTagName("text");
                    toastElement[0].AppendChild(
                        notificationXml.CreateTextNode(
                            String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationTaskStarting),
                                job.TaskName)));
                    var toastNotification = new ScheduledToastNotification(notificationXml, job.NotificationTime);
                    ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
                    job.NotificationScheduled = true;
                }

                var nextJobs =
                await
                    db.ToDoJobs.Where(toDoJob => toDoJob.IsHavingDueDate == true && 
                                        toDoJob.NotificationTime > DateTime.Now)
                        .OrderBy(toDoJob => toDoJob.NotificationTime)
                        .Take(5)
                        .ToListAsync();
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueue(true);
                updater.Clear();
                foreach (var toDoJob in nextJobs)
                {
                    XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText05);

                    var texts = tileXml.GetElementsByTagName("text");
                    texts[0].InnerText = toDoJob.TaskName;
                    texts[1].InnerText = toDoJob.NotificationTime.ToString("dddd, HH:mm");

                    // Create a new tile notification. 
                    updater.Update(new TileNotification(tileXml));
                }

                await db.Connection.UpdateAsync(job);
            }
            else
            {
                throw new Exception("Couldn't find the selected job in the database!");
            }
            NavigationService.GoBack();
        }

        public override void Init(object parameter)
        {
            _editedTask = parameter as ToDoJob;
        }
    }
}
