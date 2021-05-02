using Stuffort.Model;
using Stuffort.ViewModel.Commands;
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

namespace Stuffort.ViewModel
{
    public class NewTaskViewModel : INotifyPropertyChanged
    {
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

        public List<Subject> SubjectList { get; set; }

        public NewTaskViewModel()
        {
            SubjectList = new List<Subject>();
            InitializeSubjectList();
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
            if (string.IsNullOrEmpty(Name) || Name.Length > 120 || Name.Length < 5)
                return;

            STask Stask = new STask()
            {
                Name = Name,
                IsDeadline = IsDeadline,
                IsDone = false,
                AddedTime = DateTime.Now,
                DeadLine = Date,
                SubjectID = SubjectList[Index].ID
            };
            if (IsDeadline == false)
                Stask.DeadLine = new DateTime(1900,1,1);
            int rows;
            rows = await STaskServices.AddTask(Stask);
            if (rows > 0)
                await App.Current.MainPage.DisplayAlert("Debug", $"Task successfully saved!\nName: {Stask.Name}", "Ok");
            else
                await App.Current.MainPage.DisplayAlert("Error", "An error has occured while saving the task!", "Ok");
            await Shell.Current.GoToAsync("..");
        }
    }
}
