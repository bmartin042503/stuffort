using Stuffort.Configuration;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Stuffort.ViewModel
{
    public class SettingsViewModel
    {
        public SettingsCommand SettingsCommand { get; set; }
        public Picker LanguagePicker { get; set; }

        public List<string> Languages { get; set; }
        public SettingsViewModel(ConfigurationType ct, Picker picker)
        {
            SettingsCommand = new SettingsCommand(this, ct);
            Languages = new List<string>()
            {
                "English", "Magyar", "Polski (beta)"
            };
            LanguagePicker = picker;
            LanguagePicker.ItemsSource = Languages;
            LanguagePicker.SelectedIndex = ct.Language == "pl-PL" ? 2 : ct.Language == "hu-HU" ? 1 : 0;
        }

        public void NavigateToHomepage()
        {
            Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
