using Stuffort.Model;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Stuffort.ViewModel
{
    public class NewTaskViewModel : INotifyPropertyChanged
    {
        public STask STask { get; set; }
        public NewTaskCommand NewTaskCommand { get; set; }
        public ObservableCollection<Subject> SubjectList { get; set; }
        public NewTaskViewModel()
        {
            InitializeSubjectList();
            NewTaskCommand = new NewTaskCommand(this);
            STask = new STask();
        }

        public async void InitializeSubjectList()
        {
            var subjects = await SubjectServices.GetSubjects();
            SubjectList.Clear();
            foreach (var subject in subjects)
                SubjectList.Add(subject);
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                STask.DeadLine = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        private bool isdeadline;
        public bool IsDeadline
        {
            get { return isdeadline; }
            set
            {
                isdeadline = value;
                STask.IsDeadline = value;
                DateEnabled = value;
                OnPropertyChanged(nameof(IsDeadline));
            }
        }

        private bool dateenabled;
        public bool DateEnabled
        {
            get { return dateenabled; }
            set
            {
                dateenabled = value;
                OnPropertyChanged(nameof(DateEnabled));
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                STask.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                STask.SubjectID = SubjectList[value].ID;
                OnPropertyChanged(nameof(Index));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void SaveTask(STask s)
        {
            int rows = 0;
            rows = await STaskServices.AddTask(s);
            if (rows > 0)
                await App.Current.MainPage.DisplayAlert("Siker", $"Sikerült a feladat mentése\nNév: {s.Name}", "Ok");
            else
                await App.Current.MainPage.DisplayAlert("Hiba", "Sikertelen a mentés", "Ok");
        }
    }
}
