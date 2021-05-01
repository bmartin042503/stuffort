using Stuffort.View.ShellPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stuffort.ViewModel.Commands
{
    public class SubjectCommand : ICommand
    {
        public SubjectsViewModel SubjectsViewModel;
        public SubjectCommand(SubjectsViewModel svm)
        {
            SubjectsViewModel = svm;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await SubjectsViewModel.NavigateToNewSubject();
        }
    }
}
