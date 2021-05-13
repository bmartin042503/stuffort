using Stuffort.Model;
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
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.NameIsEmpty, "Ok");
                    return;
                }

                if (Name.Length > 120 || Name.Length < 3)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.NameLengthOverFlow, "Ok");
                    return;
                }

                int rows = 0;
                Name = RemoveSpaces(Name);
                Subject s = new Subject { Name = this.Name };
                s.AddedTime = DateTimeOffset.Now;
                rows = await SubjectServices.AddSubject(s);
                if (rows > 0)
                    await App.Current.MainPage.DisplayAlert(AppResources.Success, 
                        $"{AppResources.SubjectSuccessfullySaved}", "Ok");
                else
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        $"{AppResources.SubjectErrorWhileSaving}", "Ok");
                await Shell.Current.GoToAsync("..");
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.Error,
                $"{AppResources.ErrorMessage} {ex.Message}", "Ok"); }
        }

        public static string RemoveSpaces(string name)
        {
            if (name.StartsWith(" "))
            {
                int count = 0;
                for (int i = 0; i < name.Length; ++i)
                {
                    if (name[i] == ' ') count++;
                    else if (name[i] != ' ' && count >= 1)
                    { name = name.Substring(count); break; }
                }
            }
            if (name.EndsWith(" "))
            {
                int count = 0;
                for (int i = name.Length - 1; i >= 0; --i)
                {
                    if (name[i] == ' ') count++;
                    else if (name[i] != ' ' && count >= 1)
                    { name = name.Substring(0, name.Length - count); break; }
                }
            }
            return name;
        }
    }
}
