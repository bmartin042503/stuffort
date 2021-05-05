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
    public partial class NewSubjectPage : ContentPage
    {
        public NewSubjectViewModel NewSubjectViewModel;
        public NewSubjectPage()
        {
            InitializeComponent();
            this.NewSubjectViewModel = new NewSubjectViewModel();
            BindingContext = NewSubjectViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CurrentTitle.Text = AppResources.ResourceManager.GetString("NewSubjectPage");
            this.Title = AppResources.ResourceManager.GetString("NewSubjectPage");
            subjectNameEntry.Placeholder = AppResources.ResourceManager.GetString("Name");
            saveBtn.Text = AppResources.ResourceManager.GetString("Save");
        }
    }
}