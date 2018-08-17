using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackingApp.Models;
using TrackingApp.Services;
using TrackingApp.Views;
using Xamarin.Forms;

namespace TrackingApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        #region Attributtes
        private string email;
        private string password;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public string Email
        {
            get { return this.email; }
            set { SetValue(ref this.email, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { SetValue(ref this.password, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref isEnabled, value); }
        }
        #endregion

        #region Services
        ApiService apiService;
        DataService dataService;
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            IsEnabled = true;
            apiService = new ApiService();
            dataService = new DataService();
            navigationService = new NavigationService();
            dialogService = new DialogService();

            this.Email = "4";
            this.Password = "4";
        }
        #endregion

        #region Commands

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
        }

        private async void Login()
        {
            this.Email = this.Email?.Trim();
            this.Password = this.Password?.Trim();
            if (string.IsNullOrEmpty(this.Email))
            {
                dialogService.ShortToast("Debe indicar su correo de ingreso");
                return;
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                dialogService.ShortToast("Debe indicar su contraseña");
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var user = new User
            {
                Email = this.Email,
                Password = this.Password
            };


            user = await Autentication(user);
            if (user == null)
            {
                IsRunning = false;
                IsEnabled = true;
                return;
            }


            /*TODO: Temporal*/
            var userTemp = new User
            {
                Email = this.Email,
                Password = this.Password
            };
            user.UserId = int.Parse(userTemp.Email);
            user.UserCode = userTemp.Password;
            /*TODO: TEMPORAL*/


            user.Password = null;

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Projects = new ProjectsViewModel();
            await mainViewModel.Projects.LoadProjects(user.UserId);
            mainViewModel.Projects.ReloadProjects();
            user.TrackingRegister = mainViewModel.TrackingRegister;
            dataService.DeleteAllUsersAndInsert(user);
            mainViewModel.SetCurrentUser(user);

            var profile = ProfileViewModel.GetInstance();
            profile.GetPersonalInformation();

            navigationService.SetMainPage("ProjectsTabbedPage");


            IsRunning = false;
            IsEnabled = true;
            Email = null;
            Password = null;

            this.Password = string.Empty;
            this.Email = string.Empty;

        }

        #endregion

        public async Task<User> Autentication(User user)
        {
            try
            {
                var mainViewModel = MainViewModel.GetInstance();
                /****///
                user.UserId = int.Parse(user.Email);
                user.UserCode = user.Password;
                /****///
                user.Email = "ovalverde@dcicr.com";
                user.Password = "Sql98.67";
                //var response = await apiService.Autentication<AutenticationResult>(mainViewModel.urlBase, "api/", string.Format("AutenticacionAPI/ValidarUsuario/{0}/{1}/{2}/", user.Email, user.Password,mainViewModel.systemCode));
                //if (!response.IsSuccess)
                //{
                //    return null;
                //}
                //var result = (AutenticationResult)response.Result;
                //if (!result.ValidUser) {
                //    await dialogService.ShowMessage("Error", result.Message);
                //    return null;
                //}
                ///*TODO: Temporal*/
                //user.UserCode = result.UserCode;
                //user.FullName = result.UserName;
                user.Identification = "0304740031";//result.Identification;
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
                return null;
            }

            var userInfo = await GetUserInfo(user);
            if (userInfo == null)
            {
                return null;
            }
            user.UserId = userInfo.UserId;
            user.TrackingUse = userInfo.TrackingUse;
            return user;
        }

        public async Task<User> GetUserInfo(User user)
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<User>(modelMain.urlBase, "api/", string.Format("RecursoAPI/GetIdRecurso/{0}", user.Identification));
                if (!response.IsSuccess) { return null; }
                var result = (List<User>)response.Result;
                if (result == null || result.Count == 0 || result[0] == null || result[0].UserId == 0)
                {
                    await dialogService.ShowMessage("Error", "No se encontró información relacionada al usuario.");
                    return null;
                }
                var userResult = result[0];
                user.UserId = userResult.UserId;
                user.TrackingUse = userResult.TrackingUse;
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
                return null;
            }
            return user;//null
        }
    }


}
