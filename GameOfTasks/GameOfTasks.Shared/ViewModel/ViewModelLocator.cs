using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Ioc;
using GameOfTasks.Common;
using GameOfTasks.Common.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace GameOfTasks.ViewModel
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DefaultViewModel Default
        {
            get { return ServiceLocator.Current.GetInstance<DefaultViewModel>(); }
        }

        public AddJobViewModel AddJob
        {
            get { return ServiceLocator.Current.GetInstance<AddJobViewModel>(); }
        }

        public EditToDoViewModel EditToDo
        {
            get { return ServiceLocator.Current.GetInstance<EditToDoViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<INavigator, Navigator>();
            SimpleIoc.Default.Register<DefaultViewModel>();
            SimpleIoc.Default.Register<AddJobViewModel>();
            SimpleIoc.Default.Register<EditToDoViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
