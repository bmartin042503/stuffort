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
        public StudyTimerViewModel StudyTimerViewModel;
        public StudyTimerPage()
        {
            InitializeComponent();
            this.StudyTimerViewModel = new StudyTimerViewModel(switchTimer, taskPicker);
            BindingContext = StudyTimerViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Title = AppResources.ResourceManager.GetString("StudyTimerPage");
        }
    }
}