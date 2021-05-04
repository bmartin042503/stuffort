using Stuffort.Model;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Xamarin.Forms;
using Stuffort.Resources;
using System.ComponentModel;

namespace Stuffort.ViewModel
{
    public class TasksViewModel : INotifyPropertyChanged
    {
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
        public AsyncCommand TaskCommand { get; set; }
        public TaskRemoveCommand TaskRemoveCommand { get; set; }
        public AsyncCommand TaskRefreshCommand { get; set; }

        public TaskDoneCommand TaskDoneCommand { get; set; }

        public int SubjectListCount { get; set; }
        public ObservableCollection<Tuple<STask, STask>> TaskList { get; set; }

        public TasksViewModel()
        {
            SubjectListCount = 0;
            TaskCommand = new AsyncCommand(NavigateToNewTask);
            TaskRemoveCommand = new TaskRemoveCommand(this);
            TaskRefreshCommand = new AsyncCommand(Refresh);
            this.TaskDoneCommand = new TaskDoneCommand(this);
            TaskList = new ObservableCollection<Tuple<STask, STask>>();
        }

        public async Task Refresh()
        {
            IsRefreshing = true;
            await UpdateTasks();
            IsRefreshing = false;
        }

        public async Task UpdateTasks()
        {
            TaskList.Clear();
            var tasks = await STaskServices.GetTasks();
            var subjects = await SubjectServices.GetSubjects();
            foreach(var task in tasks)
            {
                TaskList.Add(new Tuple<STask, STask>(task, task));
            }
            SubjectListCount = (subjects as List<Subject>).Count;
        }

        public async Task NavigateToNewTask()
        {
            if (SubjectListCount == 0)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"), 
                    AppResources.ResourceManager.GetString("TaskListCountIsZero"), "Ok");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(NewTaskPage)}");
        }

    }
}
