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
    public partial class StudyTimerPage : ContentPage
    {
        private bool Running;
        public StudyTimerViewModel StudyTimerViewModel;
        public StudyTimerPage()
        {
            InitializeComponent();
            Running = false;
            this.StudyTimerViewModel = new StudyTimerViewModel(Running, switchTimer, taskPicker);
            BindingContext = this.StudyTimerViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Running == false)
            {
                await this.StudyTimerViewModel.InitializeStats();
                await this.StudyTimerViewModel.ImportTasks();
            }
            this.Title = AppResources.ResourceManager.GetString("StudyTimerPage");
        }
    }
}