// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

using Windows.UI.Xaml.Navigation;
using GameOfTasks.Common;

namespace GameOfTasks.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddJobPage : LayoutAwarePage
    {
        public AddJobPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init();
            base.OnNavigatedTo(e);
        }
    }
}
