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
        private Label NoTaskLabel;
        private SearchBar SearchBarTasks;
        public AsyncCommand TaskCommand { get; set; }
        public AsyncCommand TaskRefreshCommand { get; set; }
        public ICommand TapCommand { get; set; }
        public ICommand TaskRemoveCommand { get; set; }
        public ICommand TaskRenameCommand { get; set; }
        public ICommand TaskDoneCommand { get; set; }
        public ICommand TaskSortCommand { get; set; }
        public ICommand TaskSearchCommand { get; set; }

        public int SubjectListCount { get; set; }
        private List<STask> Tasks;

        private ObservableCollection<STask> tasklist;
        public ObservableCollection<STask> TaskList
        {
            get { return tasklist; }
            set
            {
                tasklist = value;
                OnPropertyChanged(nameof(TaskList));
            }
        }

        public TasksViewModel(Label lbl, SearchBar scb)
        {
            NoTaskLabel = lbl;
            SearchBarTasks = scb;
            TaskList = new ObservableCollection<STask>();
            SubjectListCount = 0;
            TaskCommand = new AsyncCommand(NavigateToNewTask);
            TapCommand = new Command(TapItem);
            TaskRemoveCommand = new Command(TaskRemove);
            TaskRenameCommand = new Command(TaskRename);
            TaskRefreshCommand = new AsyncCommand(Refresh);
            TaskDoneCommand = new Command(TaskDone);
            TaskSortCommand = new Command(TaskSort);
            TaskSearchCommand = new Command(TaskSearch);
        }

        public void TaskSearch(object value)
        {
            try
            {
                string searching = value as string;
                searching = searching.ToLowerInvariant();
                TaskList.Clear();
                foreach (var item in Tasks)
                {
                    if (item.Name.ToLowerInvariant().Contains(searching))
                        TaskList.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void TaskSort()
        {
            try
            {
                string sorttype = await App.Current.MainPage.DisplayActionSheet(AppResources.ResourceManager.GetString("Sorting"),
                    AppResources.ResourceManager.GetString("Cancel"), "Ok", AppResources.ResourceManager.GetString("SortByOldest"),
                    AppResources.ResourceManager.GetString("SortByNewest"), AppResources.ResourceManager.GetString("SortByCompleted"),
                    AppResources.ResourceManager.GetString("SortByUncompleted"));
                if (sorttype == AppResources.ResourceManager.GetString("SortByOldest")) TaskList = new ObservableCollection<STask>(TaskList.OrderBy(x => x.AddedTime));
                else if (sorttype == AppResources.ResourceManager.GetString("SortByNewest")) TaskList = new ObservableCollection<STask>(TaskList.OrderByDescending(x => x.AddedTime));
                else if (sorttype == AppResources.ResourceManager.GetString("SortByCompleted")) TaskList = new ObservableCollection<STask>(TaskList.OrderByDescending(x => x.IsDone));
                else if (sorttype == AppResources.ResourceManager.GetString("SortByUncompleted")) TaskList = new ObservableCollection<STask>(TaskList.OrderBy(x => x.IsDone));
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void TaskRename(object parameter)
        {
            try
            {
                STask selectedItem = parameter as STask;
                string newName = await App.Current.MainPage.DisplayPromptAsync(AppResources.ResourceManager.GetString("RenamingTask"),
                    AppResources.ResourceManager.GetString("RenameTask"),
                    "Ok", AppResources.ResourceManager.GetString("Cancel"), initialValue: selectedItem.Name);
                if (newName.Length > 120 || newName.Length < 3)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("NameLengthOverFlow"), "Ok");
                    return;
                }
                selectedItem.Name = newName;
                await STaskServices.UpdateTask(selectedItem);
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                    AppResources.ResourceManager.GetString("TaskSuccessfullyRenamed"), "Ok");
                await UpdateTasks();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void TaskDone(object parameter)
        {
            try
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
                        selectedItem.Finished = DateTime.Now;
                        await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                            AppResources.ResourceManager.GetString("TaskCompleted"), "Ok");
                        await STaskServices.UpdateTask(selectedItem);
                        await UpdateTasks();
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
                        await UpdateTasks();
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void TaskRemove(object value)
        {
            try
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
                    await UpdateTasks();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void TapItem(object value)
        {
            string name = value as string;
            if(name.Length > 14) await App.Current.MainPage.DisplayAlert("", name, "Ok");
        }

        public async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(750);
                await UpdateTasks();
                IsRefreshing = false;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async Task UpdateTasks()
        {
            try
            {
                TaskList.Clear();
                var tasks = await STaskServices.GetTasks();
                var subjects = await SubjectServices.GetSubjects();
                foreach (var task in tasks)
                {
                    TaskList.Add(task);
                }
                SubjectListCount = (subjects as List<Subject>).Count;
                if (TaskList.Count == 0)
                {
                    NoTaskLabel.Text = AppResources.ResourceManager.GetString("NoTasks");
                    NoTaskLabel.IsVisible = true;
                    SearchBarTasks.IsVisible = false;
                }
                else
                {
                    NoTaskLabel.IsVisible = false;
                    SearchBarTasks.IsVisible = true;
                }
                Tasks = (List<STask>)tasks;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async Task NavigateToNewTask()
        {
            try
            {
                if (SubjectListCount == 0)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("TaskListCountIsZero"), "Ok");
                    return;
                }

                await Shell.Current.GoToAsync($"{nameof(NewTaskPage)}");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

    }
}
