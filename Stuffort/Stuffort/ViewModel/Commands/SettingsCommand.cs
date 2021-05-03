using Stuffort.Configuration;
using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace Stuffort.ViewModel.Commands
{
    public class SettingsCommand : ICommand
    {
        public SettingsViewModel SettingsViewModel;
        public ConfigurationType ConfigurationType;
        public SettingsCommand(SettingsViewModel svm, ConfigurationType ct)
        {
            this.SettingsViewModel = svm;
            this.ConfigurationType = ct;
        }
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var picker = parameter as Picker;
            CultureInfo language = new CultureInfo(picker.SelectedIndex == 0 ? "" : picker.SelectedIndex == 1 ? "hu" : "pl");
            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = language;
            ConfigurationType.Language = language.ToString();
            ConfigurationServices.SaveConfigurationFile(ConfigurationType);
            var items = Shell.Current.Items;
            foreach (var item in items)
            {
                if (item.Route == "LoginPage")
                    continue;
                item.Title = AppResources.ResourceManager.GetString(item.Route);
            }
            App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                AppResources.ResourceManager.GetString("SettingsSaved"), "Ok");
            this.SettingsViewModel.NavigateToHomepage();
        }
    }
}
