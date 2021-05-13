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
            string searching = value as string;
            searching = searching.ToLowerInvariant();
            TaskList.Clear();
            foreach (var item in Tasks)
            {
                if (item.Name.ToLowerInvariant().Contains(searching))
                    TaskList.Add(item);
            }
        }

        public async void TaskSort()
        {
            string sorttype = await App.Current.MainPage.DisplayActionSheet(AppResources.Sorting,
                AppResources.Cancel, "Ok", AppResources.SortByOldest,
                AppResources.SortByNewest, AppResources.SortByCompleted,
                AppResources.SortByUncompleted);
            if (sorttype == AppResources.SortByOldest) TaskList = new ObservableCollection<STask>(TaskList.OrderBy(x => x.AddedTime));
            else if (sorttype == AppResources.SortByNewest) TaskList = new ObservableCollection<STask>(TaskList.OrderByDescending(x => x.AddedTime));
            else if (sorttype == AppResources.SortByCompleted) TaskList = new ObservableCollection<STask>(TaskList.OrderByDescending(x => x.IsDone));
            else if (sorttype == AppResources.SortByUncompleted) TaskList = new ObservableCollection<STask>(TaskList.OrderBy(x => x.IsDone));
        }

        public async void TaskRename(object parameter)
        {
            try
            {
                STask selectedItem = parameter as STask;
                string newName = await App.Current.MainPage.DisplayPromptAsync(AppResources.RenamingTask,
                    AppResources.RenameTask,
                    "Ok", AppResources.Cancel, initialValue: selectedItem.Name);
                if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrEmpty(newName)) return;
                if (newName.Length > 120 || newName.Length < 3)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.NameLengthOverFlow, "Ok");
                    return;
                }
                if (selectedItem.Name == newName) return;
                selectedItem.Name = newName;
                await STaskServices.UpdateTask(selectedItem);
                await App.Current.MainPage.DisplayAlert(AppResources.Success,
                    AppResources.TaskSuccessfullyRenamed, "Ok");
                await UpdateTasks();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
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
                    $"{AppResources.AreYouSureDoneTask} ({selectedItem.Name})",
                    AppResources.No, AppResources.Yes);
                    if (!setDone)
                    {
                        selectedItem.IsDone = true;
                        selectedItem.Finished = DateTime.Now;
                        await App.Current.MainPage.DisplayAlert(AppResources.Success,
                            AppResources.TaskCompleted, "Ok");
                        await STaskServices.UpdateTask(selectedItem);
                        await UpdateTasks();
                    }
                }
                else
                {
                    bool setUndone = await App.Current.MainPage.DisplayAlert("",
                    $"{AppResources.AreYouSureUndoneTask} ({selectedItem.Name})",
                    AppResources.No, AppResources.Yes);
                    if (!setUndone)
                    {
                        selectedItem.IsDone = false;
                        await App.Current.MainPage.DisplayAlert(AppResources.Success,
                            AppResources.TaskUncompleted, "Ok");
                        await STaskServices.UpdateTask(selectedItem);
                        await UpdateTasks();
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
        }

        public async void TaskRemove(object value)
        {
            try
            {
                STask selectedItem = value as STask;
                bool delete = await App.Current.MainPage.DisplayAlert(AppResources.Delete,
                    $"{AppResources.AreYouSureDeleteTask} ({selectedItem.Name})",
                    AppResources.No, AppResources.Delete);
                if (!delete)
                {
                    int rows = await STaskServices.RemoveTask(selectedItem);
                    if (rows > 0)
                        await App.Current.MainPage.DisplayAlert(AppResources.Success,
                            $"{AppResources.TaskSuccessfullyDeleted} ({selectedItem.Name})", "Ok");
                    else
                        await App.Current.MainPage.DisplayAlert(AppResources.Error,
                            $"{AppResources.TaskErrorWhileDeleting} ({selectedItem.Name})", "Ok");
                    await UpdateTasks();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
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
            await Task.Delay(750);
            await UpdateTasks();
            IsRefreshing = false;
        }

        public async Task UpdateTasks()
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
                NoTaskLabel.Text = AppResources.NoTasks;
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

        public async Task NavigateToNewTask()
        {
            if (SubjectListCount == 0)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
                    AppResources.TaskListCountIsZero, "Ok");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(NewTaskPage)}");
        }

    }
}
