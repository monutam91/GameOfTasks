using Windows.Phone.UI.Input;

namespace GameOfTasks.Common.Interfaces
{
    public interface IViewModel
    {
        void Init();

        void Init(object parameter);

        //void Init(params object[] parameters);    // Currently not used. May use it in the future

        void BackButtonPressed(object sender, BackPressedEventArgs args);
    }
}
