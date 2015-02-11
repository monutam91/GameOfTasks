using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using GalaSoft.MvvmLight.Command;
using GameOfTasks.Common;
using GameOfTasks.Model;

namespace GameOfTasks.ViewModel
{
    public class AddJobViewModel : ViewModelBase
    {
        private string _taskName;
        private int _points;
        private string _description;
        private readonly RelayCommand _createTaskCommand;
        private DateTime _notifyTime;
        private string _errorMsg;

        private bool _isHavingDueDate;

        public bool IsHavingDueDate
        {
            get { return _isHavingDueDate; }
            set
            {
                Set(() => IsHavingDueDate, ref _isHavingDueDate, value);
            }
        }

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { Set(() => ErrorMsg, ref _errorMsg, value); }
        }

        public TimeSpan NotifyTime
        {
            get { return new TimeSpan(_notifyTime.Hour, _notifyTime.Minute, _notifyTime.Second); }
            set
            {
                _notifyTime = _notifyTime.Date + value;
                RaisePropertyChanged();
            }
        }

        public DateTimeOffset NotifyDate
        {
            get { return _notifyTime; }
            set
            {
                var tmpTs = new TimeSpan(_notifyTime.Hour, _notifyTime.Minute, _notifyTime.Second);
                _notifyTime = value.DateTime;
                _notifyTime = _notifyTime.Date + tmpTs;
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
                return _taskName;
            }
            set
            {
                _taskName = value;
                if (_taskName.Length < 3 || _taskName.Length > 100)
                {
                    ErrorMsg = LocalizeHelper.GetString(LocalizationKey.TaskNameLengthError);
                }
                else
                {
                    ErrorMsg = "";
                }
                RaisePropertyChanged();
            }
        }

        public int Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                RaisePropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand;
            }
        }

        public AddJobViewModel()
        {
            TaskName = String.Empty;
            Points = 1;
            Description = String.Empty;
            _createTaskCommand = new RelayCommand(CreateTask);
            _notifyTime = DateTime.Now;
            _isHavingDueDate = false;
        }

        public override void Init()
        {
            TaskName = String.Empty;
            Points = 1;
            Description = String.Empty;
            NotifyDate = DateTime.Now;
            NotifyTime = DateTime.Now.TimeOfDay;
            IsHavingDueDate = false;
        }

        private async void CreateTask()
        {
            var newTask = new ToDoJob
            {
                TaskName = _taskName,
                HardnessPoints = _points,
                Description = _description,
                JobState = JobState.ToDo,
                NotificationTime = _notifyTime,
                IsHavingDueDate = _isHavingDueDate,
                NotificationScheduled = false,
                EarlyNotificationScheduled = false
            };

            if (IsolatedSettingsHelper.GetSettingsValue<bool>(Settings.EnableEarlyNotification))
            {
                var earlyNotify = IsolatedSettingsHelper.GetSettingsValue<TimeSpan>(Settings.EarlyNotification);
                var earlyDate = _notifyTime - earlyNotify;
                if (earlyDate.CompareTo(DateTime.Now) > 0 && earlyDate.CompareTo(DateTime.Now.AddMinutes(30)) <= 0)
                {
                    var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    var toastElement = notificationXml.GetElementsByTagName("text");
                    toastElement[0].AppendChild(
                        notificationXml.CreateTextNode(
                            String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationEarlyStarting),
                                newTask.TaskName)));
                    toastElement[1].AppendChild(
                        notificationXml.CreateTextNode(
                            String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationEarlyDescription),
                                newTask.TaskName, earlyNotify.Hours)));
                    var toastNotification = new ScheduledToastNotification(notificationXml, earlyDate);
                    ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
                    newTask.EarlyNotificationScheduled = true;
                }
            }

            if (_isHavingDueDate && _notifyTime.CompareTo(DateTime.Now) > 0 &&
                _notifyTime.CompareTo(DateTime.Now.AddMinutes(30)) <= 0)
            {
                var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                var toastElement = notificationXml.GetElementsByTagName("text");
                toastElement[0].AppendChild(
                    notificationXml.CreateTextNode(
                        String.Format(LocalizeHelper.GetString(LocalizationKey.NotificationTaskStarting),
                            newTask.TaskName)));
                var toastNotification = new ScheduledToastNotification(notificationXml, newTask.NotificationTime);
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
                newTask.NotificationScheduled = true;
            }

            var db = new DbContextAsync();
            await db.Init();
            await db.Connection.InsertAsync(newTask);

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
            foreach (var job in nextJobs)
            {
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText05);

                var texts = tileXml.GetElementsByTagName("text");
                texts[0].InnerText = job.TaskName;
                texts[1].InnerText = job.NotificationTime.ToString("dddd, HH:mm");

                // Create a new tile notification. 
                updater.Update(new TileNotification(tileXml));
            }

            NavigationService.GoBack();
        }
    }
}
