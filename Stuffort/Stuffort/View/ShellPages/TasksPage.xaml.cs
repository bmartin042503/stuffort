using Stuffort.Model;
using Stuffort.Resources;
using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffort.View.ShellPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        public TasksViewModel TasksViewModel;
        public TasksPage()
        {
            InitializeComponent();
            TasksViewModel = new TasksViewModel(noTaskLbl, searchBarTasks);
            BindingContext = TasksViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            this.Title = AppResources.ResourceManager.GetString("TasksPage");
            searchBarTasks.Placeholder = AppResources.ResourceManager.GetString("Search");
            await TasksViewModel.UpdateTasks();
        }
    }
}