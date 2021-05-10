using Stuffort.Configuration;
using Stuffort.Model;
using Stuffort.Resources;
using Stuffort.View.ShellPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stuffort.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public Command SettingsCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Picker LanguagePicker { get; set; }

        public List<string> Languages { get; set; }

        private bool notificationenabled;
        public bool NotificationEnabled
        {
            get { return notificationenabled; }
            set
            {
                notificationenabled = value;
                OnPropertyChanged(nameof(NotificationEnabled));
            }
        }

        private ConfigurationType ConfType;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetNotification()
        {
            NotificationEnabled = (ConfigurationServices.GetConfigurationData()).NotificationEnabled;
        }

        public SettingsViewModel(ConfigurationType ct, Picker picker)
        {
            SettingsCommand = new Command(SaveSettings);
            DeleteCommand = new Command(DeleteEverything);
            ConfType = ct;
            Languages = new List<string>()
            {
                "English", "Magyar", "Polski (beta)"
            };
            LanguagePicker = picker;
            LanguagePicker.ItemsSource = Languages;
        }

        public void SetLanguage()
        {
            LanguagePicker.SelectedIndex = ConfType.Language == "pl" ? 2 : ConfType.Language == "hu" ? 1 : 0;
        }

        public async void DeleteEverything()
        {
            try
            {
                bool delete = await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Warning"),
                    AppResources.ResourceManager.GetString("AreYouSureDeleteEverything"), AppResources.ResourceManager.GetString("Cancel"),
                    AppResources.ResourceManager.GetString("Yes"));
                if (!delete)
                {
                    await SubjectServices.DeleteAll();
                    await STaskServices.DeleteAll();
                    await StatisticsServices.DeleteAll();
                    await App.Current.MainPage.DisplayAlert("", AppResources.ResourceManager.GetString("DataDeleted"), "Ok");
                    await NavigateToHomepage ();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void SaveSettings(object parameter)
        {
            try
            {
                var picker = parameter as Picker;
                CultureInfo language = new CultureInfo(picker.SelectedIndex == 0 ? "" : picker.SelectedIndex == 1 ? "hu" : "pl");
                Thread.CurrentThread.CurrentUICulture = language;
                AppResources.Culture = language;
                ConfType.Language = language.ToString();
                ConfType.NotificationEnabled = NotificationEnabled;
                ConfigurationServices.SaveConfigurationFile(ConfType);
                var items = Shell.Current.Items;
                foreach (var item in items)
                {
                    if (item.Route == "LoginPage")
                        continue;
                    item.Title = AppResources.ResourceManager.GetString(item.Route);
                }
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                    AppResources.ResourceManager.GetString("SettingsSaved"), "Ok");
                await NavigateToHomepage();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),  $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async Task NavigateToHomepage()
        {
            await Shell .Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
