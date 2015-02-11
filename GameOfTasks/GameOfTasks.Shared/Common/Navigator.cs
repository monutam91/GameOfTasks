using System;
using Windows.UI.Xaml.Controls;
using GameOfTasks.Common.Interfaces;

namespace GameOfTasks.Common
{
    public sealed class Navigator : INavigator
    {

        private readonly Frame _rootFrame;

        public Navigator(Frame rootFrame)
        {
            _rootFrame = rootFrame;
        }

        public bool CanGoBack
        {
            get
            {
                return _rootFrame.CanGoBack;
            }
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                _rootFrame.GoBack();
            }
        }

        public void NavigateToViewModel<TViewModel>(object parameter = null)
        {
            var viewType = ResolveViewType<TViewModel>();
            if (parameter == null)
            {
                _rootFrame.Navigate(viewType);
            }
            else
            {
                _rootFrame.Navigate(viewType, parameter);
            }
        }

        private Type ResolveViewType<TViewModel>()
        {
            var viewModelType = typeof(TViewModel);
            var viewName = viewModelType.AssemblyQualifiedName.Replace(
              viewModelType.Name,
              viewModelType.Name.Replace("ViewModel", "Page"));
            return Type.GetType(viewName.Replace("Model", string.Empty));
        }
    }
}
