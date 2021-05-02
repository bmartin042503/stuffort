using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Stuffort.Resources;
using Xamarin.Forms;
using Stuffort.Configuration;

namespace Stuffort.ViewModel.Commands
{
    public class MainPageCommand : ICommand
    {
        public MainViewModel MainViewModel { get; set; }
        public ConfigurationType ConfigurationType { get; set; }

        public MainPageCommand(MainViewModel mainViewModel, ConfigurationType ct)
        {
            MainViewModel = mainViewModel;
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
            if (MainViewModel.AtSettingsPage == true)
                App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"), 
                    $"{AppResources.ResourceManager.GetString("LanguageSetTo")} {picker.SelectedItem as string}! {AppResources.ResourceManager.GetString("NeedReload")}", "Ok");
            MainViewModel.NavigateToHomepage();
        }
    }
}
