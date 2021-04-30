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
            cType = ConfigurationServices.GetConfigurationData();
            MainViewModel = new MainViewModel(cType);
            languageSelectionStackLayout.BindingContext = MainViewModel;
            if (cType.Language == "undefined")
            {
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                languagePicker.SelectedIndex = language == "pl" ? 3 : language == "hu" ? 2 : 1;
            }
            else
            {
                CultureInfo language = new CultureInfo(cType.Language);
                Thread.CurrentThread.CurrentUICulture = language;
                Navigation.PushModalAsync(new HomePage());
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            languagePicker.SelectedIndex = 0;
        }

        private void languagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainViewModel.MainPageCommand.Execute(sender as Picker);
        }
    }
}
