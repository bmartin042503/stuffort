using Stuffort.Model;
using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Stuffort.ViewModel.Commands
{
    public class SubjectRemoveCommand : ICommand
    {
        public SubjectsViewModel SubjectsViewModel;

        public event EventHandler CanExecuteChanged;

        public SubjectRemoveCommand(SubjectsViewModel svm)
        {
            this.SubjectsViewModel = svm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Subject selectedItem = parameter as Subject;
            int rows = await SubjectServices.RemoveSubject(selectedItem);
            if (rows > 0)
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                    $"{AppResources.ResourceManager.GetString("SubjectSuccessfullyDeleted")} ({selectedItem.Name})", "Ok");
            else
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    $"{AppResources.ResourceManager.GetString("SubjectErrorWhileDeleting")} ({selectedItem.Name})", "Ok");
            await SubjectsViewModel.UpdateSubjects();
        }
    }
}
