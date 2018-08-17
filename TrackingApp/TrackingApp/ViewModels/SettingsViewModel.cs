using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class SettingsViewModel
    {
        #region Attributes
        private DialogService dialogService;
        private NavigationService navigationService;
        #endregion

        #region Properties
        public ObservableCollection<SettingsItemViewModel> ThemesList { get; set; }
        #endregion

        #region Constructor
        public SettingsViewModel()
        {
            instance = this;
            dialogService = new DialogService();
            navigationService = new NavigationService();
            ThemesList = new ObservableCollection<SettingsItemViewModel>();
            LoadThemes();
        }
        #endregion

        #region Singleton
        private static SettingsViewModel instance;
        public static SettingsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new SettingsViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods

        public void LoadThemes()
        {
            ThemesList.Clear();
            ThemesList = new ObservableCollection<SettingsItemViewModel>
            {
                { new SettingsItemViewModel { ThemeId=1, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#001e33", SecondaryBarColor="#002d4d" } },
                { new SettingsItemViewModel { ThemeId=2, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#460623", SecondaryBarColor="#5e082f" } },                                
                { new SettingsItemViewModel { ThemeId=3, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#004d47", SecondaryBarColor="#128277" } },

                { new SettingsItemViewModel { ThemeId=4, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#746026", SecondaryBarColor="#c1a03f" } },
                { new SettingsItemViewModel { ThemeId=5, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#6d6b17", SecondaryBarColor="#b6b327" } },
                { new SettingsItemViewModel { ThemeId=6, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#463e40", SecondaryBarColor="#74676a" } },

                { new SettingsItemViewModel { ThemeId=7, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#533f56", SecondaryBarColor="#8b6990" } },
                { new SettingsItemViewModel { ThemeId=8, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#42164c", SecondaryBarColor="#6e257e" } },
                { new SettingsItemViewModel { ThemeId=9, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#685146", SecondaryBarColor="#ae8774" } },

                { new SettingsItemViewModel { ThemeId=10, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#641a44", SecondaryBarColor="#a62c71" } },
                { new SettingsItemViewModel { ThemeId=11, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#162026", SecondaryBarColor="#243640" } },
                { new SettingsItemViewModel { ThemeId=12, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#304040", SecondaryBarColor="#5b7065" } },

                { new SettingsItemViewModel { ThemeId=13, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#042d2f", SecondaryBarColor="#064346" } },
                { new SettingsItemViewModel { ThemeId=14, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#0a3300", SecondaryBarColor="#0f4d00" } },
                { new SettingsItemViewModel { ThemeId=15, IsSelected=false, BackgroundColor="#ffffff", BarTextColor="#ffffff", MainBarColor="#301b28", SecondaryBarColor="#523634" } },
            };
        }

        #endregion
    }
}
