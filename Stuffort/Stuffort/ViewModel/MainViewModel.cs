using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Stuffort.Configuration;
using Stuffort.View;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
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

        public MainPageCommand MainPageCommand { get; set; }
        public MainViewModel(ConfigurationType ct, Picker picker, string name)
        {
            this.MainPageCommand = new MainPageCommand(this, ct);
            List<string> Languages = new List<string>()
            {
                "English", "Magyar", "Polski (beta)"
            };
            LanguagePicker = picker;
            LanguagePicker.ItemsSource = Languages;
        }

        public async void NavigateToHomepage()
        {
            await Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
        }
    }
}
