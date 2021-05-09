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
            try
            {
                SubjectList.Clear();
                var subjects = await SubjectServices.GetSubjects();
                foreach (var subject in subjects)
                    SubjectList.Add(subject);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async Task SaveTask()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(Name))
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("NameIsEmpty"), "Ok");
                    return;
                }

                if (Name.Length > 120 || Name.Length < 3)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("NameLengthOverFlow"), "Ok");
                    return;
                }

                if(SubjectList[Index] == null)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("InvalidSelectedSubject"), "Ok");
                    return;
                }
                DateTime x = new DateTime(Date.Year, Date.Month, Date.Day, DateTimeSpan.Hours, DateTimeSpan.Minutes, DateTimeSpan.Seconds);
                DateTime y = DateTime.Now;
                if (y.Ticks > x.Ticks && IsDeadline == true)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("InvalidDate"), "Ok");
                    return;
                }
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
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                        $"{AppResources.ResourceManager.GetString("TaskSuccessfullySaved")}", "Ok");
                else
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("TaskErrorWhileSaving"), "Ok");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }
    }
}
