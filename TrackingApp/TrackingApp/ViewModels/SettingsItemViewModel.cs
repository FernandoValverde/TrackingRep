using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class SettingsItemViewModel : BaseViewModel
    {
        #region Attributes
        private bool isSelected;
        #endregion

        #region Properties

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                SetValue(ref this.isSelected, value);
            }
        }

        public int ThemeId { get; set; }

        public string MainBarColor { get; set; }

        public string SecondaryBarColor { get; set; }

        public string BackgroundColor { get; set; }

        public string BarTextColor { get; set; }

        #endregion Properties

        #region Services
        DataService dataService;
        #endregion

        #region Constructor
        public SettingsItemViewModel()
        {
            dataService = new DataService();
        }
        #endregion

        #region Commands

        public ICommand SelectThemeCommand
        {
            get
            {
                return new RelayCommand(SelectTheme);
            }
        }

        private void SelectTheme()
        {
            this.IsSelected = true;
            App.Current.Resources["mainBarColor"] = this.MainBarColor;
            App.Current.Resources["secondaryBarColor"] = this.SecondaryBarColor;
            App.Current.Resources["backgroundColor"] = this.BackgroundColor;
            App.Current.Resources["barTextColor"] = this.BarTextColor;
            var theme = new Theme {
                ThemeId=this.ThemeId,
                MainBarColor=this.MainBarColor,
                SecondaryBarColor=this.SecondaryBarColor,
                BackgroundColor=this.BackgroundColor,
                BarTextColor=this.BarTextColor 
            };
            dataService.DeleteAllThemesAndInsert(theme);
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.CurrentTheme = theme;
        }
        
        #endregion

    }
}
