using Stuffort.Model;
using Stuffort.View.ShellPages;
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
using System.Windows.Input;

namespace Stuffort.ViewModel
{
    public class TasksViewModel : INotifyPropertyChanged
    {
        public Label NoTaskLabel { get; set; }
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
        public AsyncCommand TaskRefreshCommand { get; set; }
        public Command TaskRemoveCommand { get; set; }
        public Command TaskRenameCommand { get; set; }
        public Command TaskDoneCommand { get; set; }

        public int SubjectListCount { get; set; }
        public ObservableCollection<Tuple<STask, STask>> TaskList { get; set; }

        public Command TapCommand { get; set; }
        public TasksViewModel(Label lbl)
        {
            NoTaskLabel = lbl;
            SubjectListCount = 0;
            TaskCommand = new AsyncCommand(NavigateToNewTask);
            TapCommand = new Command(TapItem);
            TaskRemoveCommand = new Command(TaskRemove);
            TaskRenameCommand = new Command(TaskRename);
            TaskRefreshCommand = new AsyncCommand(Refresh);
            TaskDoneCommand = new Command(TaskDone);
            TaskList = new ObservableCollection<Tuple<STask, STask>>();
        }

        public async void TaskRename(object parameter)
        {
            STask selectedItem = parameter as STask;
            string newName = await App.Current.MainPage.DisplayPromptAsync(AppResources.ResourceManager.GetString("RenamingTask"), 
                AppResources.ResourceManager.GetString("RenameTask"), 
                "Ok", AppResources.ResourceManager.GetString("Cancel"), initialValue: selectedItem.Name);
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrEmpty(newName)) return;
            if (newName.Length > 120 || newName.Length < 3) return;
            selectedItem.Name = newName;
            await STaskServices.UpdateTask(selectedItem);
            await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                AppResources.ResourceManager.GetString("TaskSuccessfullyRenamed"), "Ok");
            await Refresh();
        }

        public async void TaskDone(object parameter)
        {
            STask selectedItem = parameter as STask;
            if (selectedItem.IsDone == false)
            {
                bool setDone = await App.Current.MainPage.DisplayAlert("",
                $"{AppResources.ResourceManager.GetString("AreYouSureDoneTask")} ({selectedItem.Name})",
                AppResources.ResourceManager.GetString("No"), AppResources.ResourceManager.GetString("Yes"));
                if (!setDone)
                {
                    selectedItem.IsDone = true;
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                        AppResources.ResourceManager.GetString("TaskCompleted"), "Ok");
                    await STaskServices.UpdateTask(selectedItem);
                    await Refresh();
                }
            }
            else
            {
                bool setUndone = await App.Current.MainPage.DisplayAlert("",
                $"{AppResources.ResourceManager.GetString("AreYouSureUndoneTask")} ({selectedItem.Name})",
                AppResources.ResourceManager.GetString("No"), AppResources.ResourceManager.GetString("Yes"));
                if (!setUndone)
                {
                    selectedItem.IsDone = false;
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                        AppResources.ResourceManager.GetString("TaskUncompleted"), "Ok");
                    await STaskServices.UpdateTask(selectedItem);
                    await Refresh();
                }
            }
        }

        public async void TaskRemove(object value)
        {
            STask selectedItem = value as STask;
            bool delete = await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Delete"),
                $"{AppResources.ResourceManager.GetString("AreYouSureDeleteTask")} ({selectedItem.Name})", 
                AppResources.ResourceManager.GetString("No"), AppResources.ResourceManager.GetString("Delete"));
            if (!delete)
            {
                int rows = await STaskServices.RemoveTask(selectedItem);
                if (rows > 0)
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                        $"{AppResources.ResourceManager.GetString("TaskSuccessfullyDeleted")} ({selectedItem.Name})", "Ok");
                else
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        $"{AppResources.ResourceManager.GetString("TaskErrorWhileDeleting")} ({selectedItem.Name})", "Ok");
                await Refresh();
            }
        }

        public async void TapItem(object value)
        {
            string name = value as string;
            if(name.Length > 14) await App.Current.MainPage.DisplayAlert("", name, "Ok");
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
            if (TaskList.Count == 0)
            {
                NoTaskLabel.IsVisible = true;
                NoTaskLabel.Text = AppResources.ResourceManager.GetString("NoTasks");
            }
            else NoTaskLabel.IsVisible = false;
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
