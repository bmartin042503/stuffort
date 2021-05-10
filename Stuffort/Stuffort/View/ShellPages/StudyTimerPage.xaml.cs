using Stuffort.Resources;
using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
            this.StudyTimerViewModel = new StudyTimerViewModel(Running, switchTimer, taskPicker, timerHandlerBtn);
            BindingContext = this.StudyTimerViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var deviceInfo = DeviceDisplay.MainDisplayInfo;
            var width = deviceInfo.Width;
            if (width <= 500)
            {
                timerLbl.FontSize = 55;
                timerHandlerBtn.FontSize = 15;
                saveBtn.FontSize = 15;
                resetBtn.FontSize = 15;
            }
            await this.StudyTimerViewModel.ImportTasks();
            await this.StudyTimerViewModel.InitializeStats();
            await this.StudyTimerViewModel.ImportStats();
            freeTimerLbl.Text = AppResources.ResourceManager.GetString("FreeTimer");
            historyLbl.Text = AppResources.ResourceManager.GetString("History");
            this.Title = AppResources.ResourceManager.GetString("StudyTimerPage");
        }

        private void switchTimer_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value == true)
                taskPicker.IsEnabled = false;
            else
                taskPicker.IsEnabled = true;
        }
    }
}