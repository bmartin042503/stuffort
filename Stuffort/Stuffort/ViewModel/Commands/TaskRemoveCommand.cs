using Stuffort.Model;
using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stuffort.ViewModel.Commands
{
    public class TaskRemoveCommand : ICommand
    {
        public TasksViewModel TasksViewModel;

        public event EventHandler CanExecuteChanged;

        public TaskRemoveCommand(TasksViewModel tvm)
        {
            this.TasksViewModel = tvm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            STask selectedItem = parameter as STask;
            int rows = await STaskServices.RemoveTask(selectedItem);
            if (rows > 0)
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                    $"{AppResources.ResourceManager.GetString("TaskSuccessfullyDeleted")} ({selectedItem.Name})", "Ok");
            else
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    $"{AppResources.ResourceManager.GetString("TaskErrorWhileDeleting")} ({selectedItem.Name})", "Ok");
            await TasksViewModel.UpdateTasks();
        }
    }
}
