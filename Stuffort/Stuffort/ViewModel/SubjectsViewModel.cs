using Stuffort.Model;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Stuffort.Resources;

namespace Stuffort.ViewModel
{
    public class SubjectsViewModel : INotifyPropertyChanged
    {
        public AsyncCommand SubjectCommand { get; }
        public AsyncCommand SubjectRefreshCommand { get; }
        public SubjectRemoveCommand SubjectRemoveCommand { get; set; }
        public ObservableCollection<Subject> SubjectList { get; set; }

        private string currenttitle;
        public string CurrentTitle
        {
            get { return currenttitle; }
            set { 
                currenttitle = value;
                OnPropertyChanged(nameof(CurrentTitle));
            }
        }

        private bool isrefreshing;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsRefreshing
        {
            get { return isrefreshing; }
            set
            {
                isrefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public async Task Refresh()
        {
            IsRefreshing = true;
            await UpdateSubjects();
            IsRefreshing = false;
        }
        public SubjectsViewModel()
        {
            SubjectList = new ObservableCollection<Subject>();
            SubjectCommand = new AsyncCommand(NavigateToNewSubject);
            SubjectRemoveCommand = new SubjectRemoveCommand(this);
            SubjectRefreshCommand = new AsyncCommand(Refresh);
        }


        async Task NavigateToNewSubject()
        {
            await AppShell.Current.GoToAsync($"{nameof(NewSubjectPage)}");
        }

        public async Task UpdateSubjects()
        {
            SubjectList.Clear();
            var subjectList = await SubjectServices.GetSubjects();
            foreach (var subject in subjectList)
                SubjectList.Add(subject);
        }
    }
}
