using Stuffort.Configuration;
using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;
using Stuffort.Resources;
using Stuffort.Model;
using Xamarin.Essentials;

[assembly: ExportFont("Dosis-Book.ttf", Alias = "Dosis")]

namespace Stuffort.View.ShellPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public ConfigurationType ConfType;
        public MainViewModel MainViewModel;
        public LoginPage()
        {
            InitializeComponent();
            //File.Delete(ConfigurationServices.FilePath);
            //File.Delete(App.DatabaseLocation);
            StatisticsServices.DeleteAll();
            loginPageVersion.Text = $"{AppResources.ResourceManager.GetString("Version")} {VersionTracking.CurrentBuild} ({VersionTracking.CurrentVersion})";
            ConfType = ConfigurationServices.GetConfigurationData();
            var name = this.GetType().Name;
            MainViewModel = new MainViewModel(ConfType, languagePicker, name);
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
                AppResources.Culture = language;
                Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
            }
        }
    }
}