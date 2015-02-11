using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Windows.Phone.UI.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GameOfTasks.Common.Interfaces;

namespace GameOfTasks.Common
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {

        public static Navigator NavigationService;

        public RelayCommand GoBackCommand
        {
            get
            {
                if (!NavigationService.CanGoBack)
                    throw new InvalidOperationException("This page isn't suited to go back!");
                return new RelayCommand(GoBack);
            }
        }


        public static void SetNavigationService(Navigator service)
        {
            NavigationService = service;
        }

        protected void GoBack()
        {
            NavigationService.GoBack();
        }

        protected override void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);
        }

        public virtual void Init()
        {
            
        }

        public virtual void Init(object parameter)
        {
            
        }

        public virtual void BackButtonPressed(object sender, BackPressedEventArgs args)
        {
            if (!NavigationService.CanGoBack) return;
            NavigationService.GoBack();
            args.Handled = true;
        }
    }
}
