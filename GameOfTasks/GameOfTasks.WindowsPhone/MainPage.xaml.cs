using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GameOfTasks.Common;
using GameOfTasks.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace GameOfTasks
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            InitializeComponent();
            var trigger = new TimeTrigger(30, false);
            BackgroundExecutionManager.RequestAccessAsync();
            BackgroundAgentHelper.RegisterBackgroundTask("PhoneBackgroundAgent.NotificationBackgroundAgent",
                "NotificationAgent", trigger);
            Loaded += (sender, args) =>
            {
                var frame = (Frame) Window.Current.Content;
                ViewModelBase.SetNavigationService(new Navigator(frame));
                ViewModelBase.NavigationService.NavigateToViewModel<DefaultViewModel>();
            };
        }
    }
}
