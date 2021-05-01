using Stuffort.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Stuffort.ViewModel.Commands
{
    public class NewSubjectCommand : ICommand
    {
        public NewSubjectViewModel NewSubjectViewModel;
        public NewSubjectCommand(NewSubjectViewModel nsvm)
        {
            this.NewSubjectViewModel = nsvm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            string Name = parameter as string;
            if (string.IsNullOrEmpty(Name) || Name.Length > 50)
                return false;

            return true;

        }

        public void Execute(object parameter)
        {
            Subject s = new Subject() { Name = parameter as string, AddedTime = DateTime.Now };
            NewSubjectViewModel.SaveSubject(s);
        }
    }
}
