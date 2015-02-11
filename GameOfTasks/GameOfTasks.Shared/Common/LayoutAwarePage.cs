using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GameOfTasks.Common.Interfaces;
using GameOfTasks.ViewModel;

namespace GameOfTasks.Common
{
    public class LayoutAwarePage : Page, IView
    {

        protected LayoutAwarePage()
        {
            Window.Current.SizeChanged += WindowSizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                ViewModel.Init(e.Parameter);
            }
            ViewModel.Init();
            base.OnNavigatedTo(e);
            SetVisualState();
        }

        /// <summary>
        /// Event listener when the window size is changed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            SetVisualState();
        }

        private void SetVisualState()
        {
            var state = string.Empty;
            var applicationView = ApplicationView.GetForCurrentView();
            var size = Window.Current.Bounds;

            if (applicationView.IsFullScreen)
            {
                state = applicationView.Orientation == ApplicationViewOrientation.Landscape ? "FullScreenLandscape" : "FullScreenPortrait";
            }
            else
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (size.Width == 320.0)
                    state = "Snapped";
                else if (size.Width <= 500)
                    state = "Narrow";
                else
                    state = "Filled";
            }

            System.Diagnostics.Debug.WriteLine("Width: {0}, New VisulState: {1}",
                size.Width, state);

            VisualStateManager.GoToState(this, state, true);
        }

        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
        }
    }
}
