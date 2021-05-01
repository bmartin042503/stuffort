using System;
using System.Collections.Generic;
using System.Text;
using Stuffort.ViewModel;
using System.Windows.Input;
using Stuffort.Model;

namespace Stuffort.ViewModel.Commands
{
    public class NewTaskCommand : ICommand
    {
        public NewTaskViewModel NewTaskViewModel;
        public NewTaskCommand(NewTaskViewModel ntvm)
        {
            this.NewTaskViewModel = ntvm;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            STask s = parameter as STask;
            if ((s.Name.Length < 120 && string.IsNullOrEmpty(s.Name) == false) && (s.IsDeadline == true && s.DeadLine != null) || s.IsDeadline == false)
                return true;

            return false;
        }

        public void Execute(object parameter)
        {
            STask s = parameter as STask;
            NewTaskViewModel.SaveTask(s);
        }
    }
}
