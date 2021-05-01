using Stuffort.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Stuffort.ViewModel.Commands
{
    public class TaskCommand : ICommand
    {
        public TasksViewModel TasksViewModel;
        public TaskCommand(TasksViewModel tvm)
        {
            TasksViewModel = tvm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if(SubjectServices.GetSubjects() != null)
                return true;

            return false;
        }

        public async void Execute(object parameter)
        {
            await TasksViewModel.NavigateToNewTask();
        }
    }
}
