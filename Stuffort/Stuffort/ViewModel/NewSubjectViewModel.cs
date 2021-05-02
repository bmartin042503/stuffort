using Stuffort.Model;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using Xamarin.Forms;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Stuffort.Resources;

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
            try
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(Name))
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("NameIsEmpty"), "Ok");
                    return;
                }

                if (Name.Length > 120 || Name.Length < 5)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("NameLengthOverFlow"), "Ok");
                    return;
                }

                int rows = 0;
                Subject s = new Subject { Name = this.Name };
                s.AddedTime = DateTime.Now;
                rows = await SubjectServices.AddSubject(s);
                if (rows > 0)
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"), 
                        $"{AppResources.ResourceManager.GetString("SubjectSuccessfullySaved")} {s.Name}", "Ok");
                else
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        $"{AppResources.ResourceManager.GetString("SubjectErrorWhileSaving")} {s.Name}", "Ok");
                await Shell.Current.GoToAsync("..");
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok"); }
        }
    }
}
