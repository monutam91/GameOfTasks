using System;
using System.Collections.ObjectModel;
using Windows.Data.Xml.Dom;
using Windows.Phone.UI.Input;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using GameOfTasks.Common;
using GameOfTasks.Model;

namespace GameOfTasks.ViewModel
{
    public class DefaultViewModel : ViewModelBase
    {
        private readonly RelayCommand<RoutedEventArgs> _loaded;
        private readonly RelayCommand _addCommand;
        private readonly RelayCommand _settingsCommand;
        private bool _toDoIsVisible;
        private bool _toProgressIsVisible;
        private bool _toFinishedIsVisible;
        private int _pivotSelectedIndex;
        private readonly RelayCommand _inProgressCommand;
        private readonly RelayCommand _toDoCommand;
        private readonly RelayCommand _finishedCommand;
        private readonly RelayCommand _editToDoCommand;
        private readonly RelayCommand _deleteCommand;
        private ObservableCollection<ToDoJob> _toDoJobList;
        private ObservableCollection<ToDoJob> _inProgressJobList;
        private ObservableCollection<ToDoJob> _finishedJobList;
        private ToDoJob _selectedToDoJob;

        #region properties

        // ReSharper disable ConvertToAutoProperty

        public int NumberOfToDoTasks
        {
            get { return _toDoJobList.Count; }
        }

        public int NumberOfInProgressTasks
        {
            get { return _inProgressJobList.Count; }
        }

        public int NumberOfFinishedTasks
        {
            get { return _finishedJobList.Count; }
        }

        public RelayCommand SettingsCommand { get { return _settingsCommand; } }

        public RelayCommand EditToDoCommand
        {
            get { return _editToDoCommand; }
        }

        public RelayCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public ToDoJob SelectedToDoJob
        {
            get { return _selectedToDoJob; }
            set
            {
                _selectedToDoJob = value;
                RefreshAppBarButtons();
                RaisePropertyChanged();
                RaisePropertyChanged(() => TaskSelected);
            }
        }

        public bool TaskSelected
        {
            get { return _selectedToDoJob != null; }
        }

        public ObservableCollection<ToDoJob> FinishedJobList
        {
            get { return _finishedJobList; }
            private set
            {
                _finishedJobList = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => NumberOfFinishedTasks);
            }
        }

        public ObservableCollection<ToDoJob> InProgressJobList
        {
            get { return _inProgressJobList; }
            private set
            {
                _inProgressJobList = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => NumberOfInProgressTasks);
            }
        }

        public ObservableCollection<ToDoJob> ToDoJobList
        {
            get { return _toDoJobList; }
            private set
            {
                _toDoJobList = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => NumberOfToDoTasks);
            }
        }

        public RelayCommand FinishedCommand
        {
            get { return _finishedCommand; }
        }

        public RelayCommand ToDoCommand
        {
            get { return _toDoCommand; }
        }

        public RelayCommand InProgressCommand
        {
            get { return _inProgressCommand; }
        }

        public int PivotSelectedIndex
        {
            get { return _pivotSelectedIndex; }
            set
            {
                _pivotSelectedIndex = value;
                SelectedToDoJob = null;
                RefreshAppBarButtons();
                RaisePropertyChanged();
            }
        }

        public bool ToFinishedIsVisible
        {
            get { return _toFinishedIsVisible; }
            private set
            {
                if (value == _toFinishedIsVisible)
                {
                    return;
                }
                _toFinishedIsVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool ToProgressIsVisible
        {
            get { return _toProgressIsVisible; }
            private set
            {
                if (value == _toProgressIsVisible)
                {
                    return;
                }
                _toProgressIsVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool ToDoIsVisible
        {
            get { return _toDoIsVisible; }
            private set
            {
                if (value == _toDoIsVisible)
                {
                    return;
                }
                _toDoIsVisible = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<RoutedEventArgs> Loaded
        {
            get { return _loaded; }
        }

        public RelayCommand AddCommand
        {
            get { return _addCommand; }
        }

        // ReSharper restore ConvertToAutoProperty
        #endregion

        public DefaultViewModel()
        {
            _loaded = new RelayCommand<RoutedEventArgs>(loadedEventHandler);
            _addCommand = new RelayCommand(NavigateToAdd);
            _pivotSelectedIndex = 1;
            _selectedToDoJob = null;
            _toDoIsVisible = false;
            _toProgressIsVisible = false;
            _toFinishedIsVisible = false;
            _toDoCommand = new RelayCommand(SelectedTaskToDo);
            _finishedCommand = new RelayCommand(SelectedTaskToFinished);
            _inProgressCommand = new RelayCommand(SelectedTaskToProgress);
            _editToDoCommand = new RelayCommand(NavigateToEdit);
            _toDoJobList = new ObservableCollection<ToDoJob>();
            _inProgressJobList = new ObservableCollection<ToDoJob>();
            _finishedJobList = new ObservableCollection<ToDoJob>();
            _settingsCommand = new RelayCommand(NavigateToSettings);
            _deleteCommand = new RelayCommand(DeleteSelectedTask);
        }

        public override void BackButtonPressed(object sender, BackPressedEventArgs args)
        {
            args.Handled = true;
            Application.Current.Exit();
        }

        private void RefreshAppBarButtons()
        {
            var toFinishedIsVisible = false;
            var toProgressIsVisible = false;
            // ReSharper disable ReplaceWithSingleAssignment.False
            var toDoIsVisible = false;
            if (SelectedToDoJob != null && SelectedToDoJob.JobState == JobState.Finished && PivotSelectedIndex == (int)PivotPages.FinishedPage)
            {
                toDoIsVisible = true;
            }
            // ReSharper restore ReplaceWithSingleAssignment.False
            if (SelectedToDoJob != null && SelectedToDoJob.JobState == JobState.InProgress && PivotSelectedIndex == (int)PivotPages.InProgressPage)
            {
                toFinishedIsVisible = true;
                toDoIsVisible = true;
            }
            if (SelectedToDoJob != null && SelectedToDoJob.JobState == JobState.ToDo && PivotSelectedIndex == (int)PivotPages.ToDoPage)
            {
                toFinishedIsVisible = true;
                toProgressIsVisible = true;
                toDoIsVisible = false;
            }
            ToFinishedIsVisible = toFinishedIsVisible;
            ToProgressIsVisible = toProgressIsVisible;
            ToDoIsVisible = toDoIsVisible;
        }

        private async void DeleteSelectedTask()
        {
            var selectedTask = SelectedToDoJob;
            DeleteTaskFromCorrectList(selectedTask);
            var db = new DbContextAsync();
            await db.Init();
            await db.Connection.DeleteAsync(selectedTask);
        }

        private async void SelectedTaskToDo()
        {
            var selectedTask = SelectedToDoJob;
            DeleteTaskFromCorrectList(selectedTask);
            ToDoJobList.Add(selectedTask);
            var db = new DbContextAsync();
            await db.Init();
            var job =
                await
                    db.ToDoJobs.Where(task => task.JobId == selectedTask.JobId).FirstOrDefaultAsync();
            if (job != null)
            {
                job.JobState = JobState.ToDo;
                await db.Connection.UpdateAsync(job);
                SelectedToDoJob = null;
                RefreshAllJobs();
            }
            else
            {
                throw new Exception("Couldn't find the selected job in the database!");
            }
        }

        private async void SelectedTaskToFinished()
        {
            var selectedTask = SelectedToDoJob;
            DeleteTaskFromCorrectList(selectedTask);
            FinishedJobList.Add(selectedTask);
            var db = new DbContextAsync();
            await db.Init();
            var job =
                await
                    db.ToDoJobs.Where(task => task.JobId == selectedTask.JobId).FirstOrDefaultAsync();
            if (job != null)
            {
                job.JobState = JobState.Finished;
                await db.Connection.UpdateAsync(job);
                SelectedToDoJob = null;
                RefreshAllJobs();
            }
            else
            {
                throw new Exception("Couldn't find the selected job in the database!");
            }
        }

        private async void SelectedTaskToProgress()
        {
            var selectedTask = SelectedToDoJob;
            DeleteTaskFromCorrectList(selectedTask);
            InProgressJobList.Add(selectedTask);
            var db = new DbContextAsync();
            await db.Init();
            var job =
                await
                    db.ToDoJobs.Where(task => task.JobId == selectedTask.JobId).FirstOrDefaultAsync();
            if (job != null)
            {
                job.JobState = JobState.InProgress;
                await db.Connection.UpdateAsync(job);
                RefreshAllJobs();
            }
            else
            {
                throw new Exception("Couldn't find the selected job in the database!");
            }
        }

        private void DeleteTaskFromCorrectList(ToDoJob task)
        {
            if (task.JobState == JobState.Finished)
            {
                FinishedJobList.Remove(task);
                RaisePropertyChanged(() => NumberOfFinishedTasks);
            }
            else if (task.JobState == JobState.ToDo)
            {
                ToDoJobList.Remove(task);
                RaisePropertyChanged(() => NumberOfToDoTasks);
            }
            else if (task.JobState == JobState.InProgress)
            {
                InProgressJobList.Remove(task);
                RaisePropertyChanged(() => NumberOfInProgressTasks);
            }
        }

        private void NavigateToAdd()
        {
            NavigationService.NavigateToViewModel<AddJobViewModel>();
        }

        private void loadedEventHandler(RoutedEventArgs obj)
        {
            Init();
        }

        public override async void Init()
        {
            RefreshAllJobs();
            var db = new DbContextAsync();
            await db.Init();
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
        }

        private async void RefreshAllJobs()
        {
            var db = new DbContextAsync();
            await db.Init();
            ToDoJobList =
                new ObservableCollection<ToDoJob>(
                    await db.ToDoJobs.Where(job => job.JobState == JobState.ToDo).ToListAsync());
            InProgressJobList =
                new ObservableCollection<ToDoJob>(
                    await db.ToDoJobs.Where(job => job.JobState == JobState.InProgress).ToListAsync());
            FinishedJobList =
                new ObservableCollection<ToDoJob>(
                    await db.ToDoJobs.Where(job => job.JobState == JobState.Finished).ToListAsync());
        }

        private void NavigateToEdit()
        {
            NavigationService.NavigateToViewModel<EditToDoViewModel>(SelectedToDoJob);
        }

        private void NavigateToSettings()
        {
            NavigationService.NavigateToViewModel<SettingsViewModel>();
        }
    }

    public enum PivotPages { FinishedPage, ToDoPage, InProgressPage }
}
