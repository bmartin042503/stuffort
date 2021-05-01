using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Xamarin.Forms;
using Stuffort.Configuration;
using Stuffort.View;
using System.IO;
using Stuffort.Resources;

namespace Stuffort
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel MainViewModel;
        private ConfigurationType ConfType;
        public MainPage()
        {
            InitializeComponent();
            File.Delete(ConfigurationServices.FilePath); //TÖRLENDŐ
            ConfType = ConfigurationServices.GetConfigurationData();
            MainViewModel = new MainViewModel(ConfType, languagePicker);
            languageSelectionStackLayout.BindingContext = MainViewModel;
            if (ConfType.Language == "undefined")
            {
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                languagePicker.SelectedIndex = language == "pl-PL" ? 2 : language == "hu-HU" ? 1 : 0;
            }
            else
            {
                CultureInfo language = new CultureInfo(ConfType.Language);
                Thread.CurrentThread.CurrentUICulture = language;
                App.Current.MainPage = new HomeShell();
            }
        }
    }
}
