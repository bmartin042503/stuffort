using Stuffort.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Xamarin.Forms;
using Stuffort.Resources;

namespace Stuffort.ViewModel
{
    public class NewTaskViewModel : INotifyPropertyChanged
    {
        public DateTime DateTimeNow { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if(value != name)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index)
                {
                    index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private bool isdeadline;
        public bool IsDeadline
        {
            get { return isdeadline; }
            set
            {
                if (value != isdeadline)
                {
                    isdeadline = value;
                    OnPropertyChanged(nameof(IsDeadline));
                }
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if(value != date)
                {
                    date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        private TimeSpan datetimespan;
        public TimeSpan DateTimeSpan
        {
            get { return datetimespan; }
            set
            {
                if(value != datetimespan)
                {
                    datetimespan = value;
                    OnPropertyChanged(nameof(DateTimeSpan));
                }
            }
        }

        public ObservableCollection<Subject> SubjectList { get; set; }
        public AsyncCommand NewTaskCommand { get; set; }
        public NewTaskViewModel()
        {
            SubjectList = new ObservableCollection<Subject>();
            InitializeSubjectList();
            DateTimeNow = DateTime.Now;
            DateTimeSpan = DateTimeNow.TimeOfDay;
            Date = DateTimeNow;
            this.Index = 0;
            NewTaskCommand = new AsyncCommand(SaveTask);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void InitializeSubjectList()
        {
            SubjectList.Clear();
            var subjects = await SubjectServices.GetSubjects();
            foreach (var subject in subjects)
                SubjectList.Add(subject);
        }

        public async Task SaveTask()
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

                if(SubjectList[Index] == null)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.InvalidSelectedSubject, "Ok");
                    return;
                }
                DateTime x = new DateTime(Date.Year, Date.Month, Date.Day, DateTimeSpan.Hours, DateTimeSpan.Minutes, DateTimeSpan.Seconds);
                DateTime y = DateTime.Now;
                if (y.Ticks > x.Ticks && IsDeadline == true)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.InvalidDate, "Ok");
                    return;
                }
                Name = RemoveSpaces(Name);
                STask Stask = new STask()
                {
                    Name = Name,
                    IsDeadline = IsDeadline,
                    IsDone = false,
                    AddedTime = DateTimeOffset.Now,
                    DeadLine = x,
                    SubjectID = SubjectList[Index].ID,
                    SubjectName = SubjectList[Index].Name
                };
                if (IsDeadline == false)
                    Stask.DeadLine = new DateTimeOffset(new DateTime(1900,1,1));
                int rows;
                rows = await STaskServices.AddTask(Stask);
                if (rows > 0)
                    await App.Current.MainPage.DisplayAlert(AppResources.Success,
                        $"{AppResources.TaskSuccessfullySaved}", "Ok");
                else
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.TaskErrorWhileSaving, "Ok");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        $"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
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
