using Stuffort.Model;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;

namespace Stuffort.ViewModel
{
    public class NewSubjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public AsyncCommand SubjectCommand { get; set; }
        public NewSubjectViewModel()
        {
            SubjectCommand = new AsyncCommand(SaveSubject);
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

        public async Task SaveSubject()
        {
            if (string.IsNullOrEmpty(Name) || Name.Length < 3 || Name.Length > 50)
                return;

            int rows = 0;
            Subject s = new Subject { Name = this.Name };
            rows = await SubjectServices.AddSubject(s);
            if(rows > 0)
                await App.Current.MainPage.DisplayAlert("Siker", $"Sikerült a tantárgy mentése\nNév: {s.Name}", "Ok");
            else
                await App.Current.MainPage.DisplayAlert("Hiba", "Sikertelen a mentés", "Ok");
        }
    }
}
