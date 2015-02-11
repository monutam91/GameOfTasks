namespace GameOfTasks.Common.Interfaces
{
    public interface INavigator
    {
        bool CanGoBack { get; }
        void GoBack();
        void NavigateToViewModel<TViewModel>(object parameter = null);
    }
}
