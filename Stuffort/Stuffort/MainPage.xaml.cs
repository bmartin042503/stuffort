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
        private ConfigurationType cType;
        public MainPage()
        {
            InitializeComponent();
            File.Delete(ConfigurationServices.FilePath);
            cType = ConfigurationServices.GetConfigurationData();
            MainViewModel = new MainViewModel(cType);
            languageSelectionStackLayout.BindingContext = MainViewModel;
            if (cType.Language == "undefined")
            {
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                languagePicker.SelectedIndex = language == "pl" ? 2 : language == "hu" ? 1 : 0;
            }
            else
            {
                CultureInfo language = new CultureInfo(cType.Language);
                Thread.CurrentThread.CurrentUICulture = language;
                Navigation.PushModalAsync(new HomePage());
            }
        }

        private void languagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainViewModel.MainPageCommand.Execute(sender as Picker);
        }
    }
}
