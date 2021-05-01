using System;
using System.Collections.Generic;
using System.Text;
using Stuffort.Configuration;
using Stuffort.View;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using Xamarin.Forms;

namespace Stuffort.ViewModel
{
    public class MainViewModel
    {
        public Picker LanguagePicker { get; set; }
        public List<string> Languages { get; set; }
        public MainPageCommand MainPageCommand { get; set; }
        public MainViewModel(ConfigurationType ct, Picker picker)
        {
            this.MainPageCommand = new MainPageCommand(this, ct);
            Languages = new List<string>()
            {
                "English", "Magyar", "Polski"
            };
            LanguagePicker = picker;
        }

        public void NavigateToHomepage()
        {
            Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
