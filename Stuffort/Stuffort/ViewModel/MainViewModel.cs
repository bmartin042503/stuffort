using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using Stuffort.Configuration;
using Stuffort.Resources;
using Stuffort.View;
using Stuffort.View.ShellPages;
using Xamarin.Forms;

namespace Stuffort.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Picker LanguagePicker { get; set; }
        public Command MainPageCommand { get; set; }

        public ConfigurationType ConfType { get; set; }
        public MainViewModel(ConfigurationType ct, Picker picker)
        {
            List<string> Languages = new List<string>()
            {
                "English", "Magyar", "Polski (beta)"
            };
            MainPageCommand = new Command(LanguageSetting);
            ConfType = ct;
            LanguagePicker = picker;
            LanguagePicker.ItemsSource = Languages;
        }

        public void LanguageSetting(object value)
        {
            try
            {
                var picker = value as Picker;
                CultureInfo language = new CultureInfo(picker.SelectedIndex == 0 ? "" : picker.SelectedIndex == 1 ? "hu" : "pl");
                Thread.CurrentThread.CurrentUICulture = language;
                AppResources.Culture = language;
                ConfType.Language = language.ToString();
                ConfType.NotificationEnabled = true;
                ConfigurationServices.SaveConfigurationFile(ConfType);
                var items = Shell.Current.Items;
                foreach (var item in items)
                {
                    if (item.Route == "LoginPage")
                        continue;
                    item.Title = AppResources.ResourceManager.GetString(item.Route);
                }
                NavigateToHomepage();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void NavigateToHomepage()
        {
            await Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
