using Stuffort.Model;
using Stuffort.View.ShellPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Stuffort.Resources;

namespace Stuffort.ViewModel
{
    public class SubjectsViewModel : INotifyPropertyChanged
    {
        public Label NoSubjectLabel { get; set; }
        public AsyncCommand SubjectCommand { get; set; }
        public AsyncCommand SubjectRefreshCommand { get; set; }
        public Command SubjectRemoveCommand { get; set; }
        public Command SubjectRenameCommand { get; set; }
        public Command TapCommand { get; set; }
        public ObservableCollection<Tuple<Subject, string>> SubjectList { get; set; }

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
        public async void TapItem(object value)
        {
            try
            {
                string name = value as string;
                if (name.Length > 14) await App.Current.MainPage.DisplayAlert("", name, "Ok");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async Task Refresh()
        {
            IsRefreshing = true;
            await UpdateSubjects();
            IsRefreshing = false;
        }
        public SubjectsViewModel(Label lbl)
        {
            NoSubjectLabel = lbl;
            SubjectList = new ObservableCollection<Tuple<Subject,string>>();
            TapCommand = new Command(TapItem);
            SubjectCommand = new AsyncCommand(NavigateToNewSubject);
            SubjectRemoveCommand = new Command(RemovingSubject);
            SubjectRefreshCommand = new AsyncCommand(Refresh);
            SubjectRenameCommand = new Command(SubjectRename);
        }

        public async void SubjectRename(object parameter)
        {
            try
            {
                Subject selectedItem = parameter as Subject;
                string newName = await App.Current.MainPage.DisplayPromptAsync(AppResources.ResourceManager.GetString("RenamingSubject"),
                    AppResources.ResourceManager.GetString("RenameSubject"),
                    "Ok", AppResources.ResourceManager.GetString("Cancel"), initialValue: selectedItem.Name);
                if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrEmpty(newName)) return;
                if (newName.Length > 120 || newName.Length < 3)
                {
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                        AppResources.ResourceManager.GetString("NameLengthOverFlow"), "Ok");
                    return;
                }
                if (selectedItem.Name == newName) return;
                selectedItem.Name = newName;
                await SubjectServices.RenameSubject(selectedItem, newName);
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                    AppResources.ResourceManager.GetString("SubjectSuccessfullyRenamed"), "Ok");
                await UpdateSubjects();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void RemovingSubject(object parameter)
        {
            try
            {
                Subject selectedItem = parameter as Subject;
                bool delete = await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Delete"),
                    $"{AppResources.ResourceManager.GetString("AreYouSureDeleteSubject")} ({selectedItem.Name})",
                    AppResources.ResourceManager.GetString("No"), AppResources.ResourceManager.GetString("Delete"));
                if (!delete)
                {
                    int rows = await SubjectServices.RemoveSubject(selectedItem);
                    if (rows > 0)
                        await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Success"),
                            $"{AppResources.ResourceManager.GetString("SubjectSuccessfullyDeleted")} ({selectedItem.Name})", "Ok");
                    else
                        await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                            $"{AppResources.ResourceManager.GetString("SubjectErrorWhileDeleting")} ({selectedItem.Name})", "Ok");
                    await UpdateSubjects();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }
        async Task NavigateToNewSubject()
        {
            await AppShell.Current.GoToAsync($"{nameof(NewSubjectPage)}");
        }

        public async Task UpdateSubjects()
        {
            try
            {
                SubjectList.Clear();
                var subjectList = await SubjectServices.GetSubjects();
                var tasksList = await STaskServices.GetTasks();
                foreach (var subject in subjectList)
                {
                    var countOfTasks = string.Format($"{AppResources.ResourceManager.GetString("CountOfTasks")} {tasksList.Where(x => x.SubjectID == subject.ID).Count()}");
                    SubjectList.Add(new Tuple<Subject, string>(subject, countOfTasks));
                }
                if (SubjectList.Count == 0)
                {
                    NoSubjectLabel.IsVisible = true;
                    NoSubjectLabel.Text = AppResources.ResourceManager.GetString("NoSubjects");
                }
                else NoSubjectLabel.IsVisible = false;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }
    }
}
