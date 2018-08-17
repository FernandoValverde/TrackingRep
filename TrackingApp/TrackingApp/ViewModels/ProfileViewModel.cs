using System.IO;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackingApp.Models;
using TrackingApp.Services;
using Xamarin.Forms;

namespace TrackingApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        #region Attributes
        private string email;
        private string fullName;
        private string identification;
        private string officePhone;
        private string admissionDate;
        private string state;
        private string category;
        private string office;
        private bool isRefreshing;
        private ImageSource photo;
        #endregion Attributes

        #region Properties

        public string Email
        {
            set { SetValue<string>(ref email, value); }
            get { return email; }
        }

        public string FullName
        {
            set { SetValue<string>(ref fullName, value); }
            get { return fullName; }
        }

        public string Identification
        {
            set { SetValue<string>(ref identification, value); }
            get { return identification; }
        }

        public string OfficePhone
        {
            set { SetValue<string>(ref officePhone, value); }
            get { return officePhone; }
        }

        public string AdmissionDate
        {
            set { SetValue<string>(ref admissionDate, value); }
            get { return admissionDate; }
        }

        public string State
        {
            set { SetValue<string>(ref state, value); }
            get { return state; }
        }

        public string Category
        {
            set { SetValue<string>(ref category, value); }
            get { return category; }
        }

        public string Office
        {
            set { SetValue<string>(ref office, value); }
            get { return office; }
        }

        public bool IsRefreshing
        {
            set { SetValue<bool>(ref isRefreshing, value); }
            get { return isRefreshing; }
        }

        public ImageSource Photo
        {
            set { SetValue<ImageSource>(ref photo, value); }
            get { return photo; }
        }

        #endregion

        #region Contructor
        public ProfileViewModel()
        {
            instance = this;
        }
        #endregion

        #region Singleton
        private static ProfileViewModel instance;
        public static ProfileViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ProfileViewModel();
            }
            return instance;
        }
        #endregion

        public async Task<bool> GetPersonalInformation()
        {
            var apiService = new ApiService();
            var dialogService = new DialogService();
            var navigationService = new NavigationService();
            var dataService = new DataService();

            var modelMain = MainViewModel.GetInstance();
            this.Email = modelMain.CurrentUser.Email;
            this.Identification = modelMain.CurrentUser.Identification;

            this.FullName = modelMain.CurrentUser.FullName;
            this.OfficePhone = modelMain.CurrentUser.OfficePhone;
            this.AdmissionDate = modelMain.CurrentUser.AdmissionDate;
            this.State = modelMain.CurrentUser.State == true ? "Activo" : modelMain.CurrentUser.State == false ? "Inactivo" : "";
            this.Category = modelMain.CurrentUser.Category;
            this.Office = modelMain.CurrentUser.Office;

            byte[] photoByte;
            if (modelMain.CurrentUser.Photo !=null && modelMain.CurrentUser.Photo.Length>0)
            {
                photoByte = (byte[])modelMain.CurrentUser.Photo;
                Stream stream = new MemoryStream(photoByte);
                this.Photo = ImageSource.FromStream(() => stream);
            }
            else
            {
                var Avatar = new Image { };
                Avatar.Source = "profile.png";
                this.Photo = Avatar.Source;
            }


            if (string.IsNullOrEmpty(this.FullName))
            {
                IsRefreshing = true;
            }
            try
            {
                var response = await apiService.Get<PersonalInformation>(modelMain.urlBase, "/api", string.Format("/RecursoAPI/{0}", modelMain.CurrentUser.UserId));
                if (!response.IsSuccess || response.Result == null) { return false; }

                var userInformation = ((List<PersonalInformation>)response.Result).FirstOrDefault();

                modelMain.CurrentUser.FullName = userInformation.FullName;
                modelMain.CurrentUser.OfficePhone = userInformation.OfficePhone;
                modelMain.CurrentUser.AdmissionDate = userInformation.AdmissionDate;
                modelMain.CurrentUser.State = userInformation.State;
                modelMain.CurrentUser.Category = userInformation.Category.Description;
                modelMain.CurrentUser.Office = userInformation.Office.Name;

                this.FullName = modelMain.CurrentUser.FullName;
                this.OfficePhone = modelMain.CurrentUser.OfficePhone;
                this.AdmissionDate = modelMain.CurrentUser.AdmissionDate;
                this.State = modelMain.CurrentUser.State == true ? "Activo" : "Inactivo"; ;
                this.Category = modelMain.CurrentUser.Category;
                this.Office = modelMain.CurrentUser.Office;

                var result = await apiService.GetOne<byte[]>(modelMain.urlBase, "/api", string.Format("/FotografiaAPI/{0}", modelMain.CurrentUser.UserId));
                
                if (result.IsSuccess)
                {
                    if (result.Result != null)
                    {
                        var imageSource = (byte[])result.Result;
                        if (imageSource.Length > 0 && !CompareBytesArray(imageSource, modelMain.CurrentUser.Photo))
                        {
                            photoByte = imageSource;
                            Stream stream = new MemoryStream(photoByte);
                            this.Photo = ImageSource.FromStream(() => stream);
                            modelMain.CurrentUser.Photo = photoByte;
                        }
                    }
                }

                dataService.DeleteAllUsersAndInsert(modelMain.CurrentUser);
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            IsRefreshing = false;
            return true;
        }

        private static bool CompareBytesArray(byte[] strA, byte[] strB)
        {
            if (strA == null || strB == null)
                return false;

            int length = strA.Length;
            if (length != strB.Length)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (strA[i] != strB[i]) return false;
            }
            return true;
        }

        #region Commands
        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); } }

        public async void Refresh()
        {
            await GetPersonalInformation();
        }
        #endregion Commands
    }
}
