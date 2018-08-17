using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class ProjectItemViewModel:Project
    {
        #region Commands

        public ICommand SelectProjectCommand
        {
            get
            {
                return new RelayCommand(SelectProject);
            }
        }

        private async void SelectProject()
        {
            NavigationService navigationService = new NavigationService();
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.ProjectSelected = this.ProjectId;
            await navigationService.Navigate("TasksPage");

        }
        #endregion Commands
    }
}
