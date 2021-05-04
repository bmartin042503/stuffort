using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Stuffort.Resources;

namespace Stuffort.View.ShellPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTaskPage : ContentPage
    {
        public NewTaskViewModel NewTaskViewModel;
        public NewTaskPage()
        {
            InitializeComponent();
            NewTaskViewModel = new NewTaskViewModel();
            BindingContext = NewTaskViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CurrentTitle.Text = AppResources.ResourceManager.GetString("NewTaskPage");
            this.Title = AppResources.ResourceManager.GetString("NewTaskPage");
            chooseSubjectlbl.Text = AppResources.ResourceManager.GetString("ChooseSubject");
            deadLinelbl.Text = AppResources.ResourceManager.GetString("Deadline");
            subjectPicker.Title = AppResources.ResourceManager.GetString("ChooseSubjectImproved");
        }
    }
}