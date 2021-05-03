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
    public partial class SubjectsPage : ContentPage
    {
        public SubjectsViewModel SubjectsViewModel;
        public SubjectsPage()
        {
            InitializeComponent();
            SubjectsViewModel = new SubjectsViewModel();
            BindingContext = SubjectsViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            CurrentTitle.Text = AppResources.ResourceManager.GetString("SubjectsPage");
            await SubjectsViewModel.UpdateSubjects();
        }
    }
}