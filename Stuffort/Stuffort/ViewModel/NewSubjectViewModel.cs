using Stuffort.Model;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Stuffort.ViewModel
{
    public class NewSubjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public NewSubjectCommand SubjectCommand { get; set; }
        public NewSubjectViewModel()
        {
            SubjectCommand = new NewSubjectCommand(this);
        }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void SaveSubject(Subject s)
        {
            int rows = 0;
            rows = await SubjectServices.AddSubject(s);
            if(rows > 0)
                await App.Current.MainPage.DisplayAlert("Siker", $"Sikerült a tantárgy mentése\nNév: {s.Name}", "Ok");
            else
                await App.Current.MainPage.DisplayAlert("Hiba", "Sikertelen a mentés", "Ok");
        }
    }
}
