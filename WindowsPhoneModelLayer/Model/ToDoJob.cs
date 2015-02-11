using System;
using SQLite;

namespace GameOfTasks.Model
{
    [Table("Jobs")]
    public class ToDoJob
    {

        [PrimaryKey, AutoIncrement]
        public int JobId { get; set; }

        [NotNull, MaxLength(100), Indexed]
        public string TaskName { get; set; }

        [NotNull]
        public int HardnessPoints { get; set; }

        public string Description { get; set; }

        [NotNull]
        public JobState JobState { get; set; }

        [Column("NotificationTime")]
        public DateTime NotificationTime { get; set; }

        [NotNull]
        public bool IsHavingDueDate { get; set; }

        [NotNull]
        public bool NotificationScheduled { get; set; }

        [NotNull]
        public bool EarlyNotificationScheduled { get; set; }

        public ToDoJob()
        {
            TaskName = String.Empty;
            HardnessPoints = 1;
            Description = String.Empty;
            JobState = JobState.ToDo;
            NotificationTime = DateTime.Now;
            IsHavingDueDate = false;
            NotificationScheduled = false;
            EarlyNotificationScheduled = false;
        }
    }

    public enum JobState { ToDo, InProgress, Finished }
}
