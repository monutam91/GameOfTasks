using Windows.ApplicationModel.Background;

namespace GameOfTasks.Common
{
    public static class BackgroundAgentHelper
    {
        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string name,
            IBackgroundTrigger trigger, IBackgroundCondition con = null
            )
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            var builder = new BackgroundTaskBuilder {Name = name, TaskEntryPoint = taskEntryPoint};
            builder.SetTrigger(trigger);

            if (con != null)
            {
                builder.AddCondition(con);
            }
            var task = builder.Register();
            return task;
        }
    }
}
