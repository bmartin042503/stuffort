using Stuffort.Configuration;
using Stuffort.Resources;
using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffort.View.ShellPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsViewModel SettingsViewModel;
        public SettingsPage()
        {
            InitializeComponent();
            SettingsViewModel = new SettingsViewModel(ConfigurationServices.GetConfigurationData(), languagePicker);
            languagePicker.ItemsSource = new List<string>()
            {
                "English", "Magyar", "Polski"
            };
            languagePicker.SelectedIndex = 0;
            BindingContext = this.SettingsViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CultureInfo cultureInfo = AppResources.Culture;
            this.Title = AppResources.SettingsPage;
            saveBtn.Text = AppResources.Save;
            languageLbl.Text = AppResources.Language;
            deleteBtn.Text = AppResources.DeleteEverything;
            notificationLbl.Text = AppResources.Notifications;
            this.SettingsViewModel.SetNotification();
            this.SettingsViewModel.SetLanguage();
        }
    }
}