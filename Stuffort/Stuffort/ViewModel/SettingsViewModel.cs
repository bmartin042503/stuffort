using Stuffort.Configuration;
using Stuffort.Resources;
using Stuffort.View.ShellPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Stuffort.ViewModel
{
    public class SettingsViewModel
    {
        public Command SettingsCommand { get; set; }
        public Picker LanguagePicker { get; set; }

        public List<string> Languages { get; set; }

        public ConfigurationType ConfType { get; set; }
        public SettingsViewModel(ConfigurationType ct, Picker picker)
        {
            SettingsCommand = new Command(SaveSettings);
            ConfType = ct;
            Languages = new List<string>()
            {
                "English", "Magyar", "Polski (beta)"
            };
            LanguagePicker = picker;
            LanguagePicker.ItemsSource = Languages;
            LanguagePicker.SelectedIndex = ct.Language == "pl" ? 2 : ct.Language == "hu" ? 1 : 0;
        }

        public void SaveSettings(object parameter)
        {
            var picker = parameter as Picker;
            CultureInfo language = new CultureInfo(picker.SelectedIndex == 0 ? "" : picker.SelectedIndex == 1 ? "hu" : "pl");
            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = language;
            ConfType.Language = language.ToString();
            ConfigurationServices.SaveConfigurationFile(ConfType);
            var items = Shell.Current.Items;
            foreach (var item in items)
            {
                if (item.Route == "LoginPage")
                    continue;
                item.Title = AppResources.ResourceManager.GetString(item.Route);
            }
            App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                AppResources.ResourceManager.GetString("SettingsSaved"), "Ok");
            NavigateToHomepage();
        }

        public void NavigateToHomepage()
        {
            Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
