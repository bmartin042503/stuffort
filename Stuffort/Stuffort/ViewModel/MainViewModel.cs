using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Stuffort.Configuration;
using Stuffort.View;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using Xamarin.Forms;

namespace Stuffort.ViewModel
{
    public class MainViewModel
    {
        public bool AtSettingsPage;
        public Picker LanguagePicker { get; set; }
        public List<string> Languages { get; set; }
        public MainPageCommand MainPageCommand { get; set; }
        public MainViewModel(ConfigurationType ct, Picker picker, string name)
        {
            this.MainPageCommand = new MainPageCommand(this, ct);
            Languages = new List<string>()
            {
                "English", "Magyar", "Polski (beta)"
            };
            LanguagePicker = picker;
            if (name == "SettingsPage")
            {
                AtSettingsPage = true;
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                LanguagePicker.SelectedIndex = language == "pl-PL" ? 2 : language == "hu-HU" ? 1 : 0;
            }
            else
                AtSettingsPage = false;
        }

        public void NavigateToHomepage()
        {
            Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
