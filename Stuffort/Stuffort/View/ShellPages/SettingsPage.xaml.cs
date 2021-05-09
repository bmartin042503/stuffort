using Stuffort.Configuration;
using Stuffort.Resources;
using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
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
            this.Title = AppResources.ResourceManager.GetString("SettingsPage");
            saveBtn.Text = AppResources.ResourceManager.GetString("Save");
            languageLbl.Text = AppResources.ResourceManager.GetString("Language");
            deleteBtn.Text = AppResources.ResourceManager.GetString("DeleteEverything");
            notificationLbl.Text = AppResources.ResourceManager.GetString("Notifications");
            this.SettingsViewModel.SetNotification();
        }
    }
}