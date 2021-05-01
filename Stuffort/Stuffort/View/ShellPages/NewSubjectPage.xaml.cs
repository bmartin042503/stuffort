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
            NewSubjectViewModel = new NewSubjectViewModel();
            newSubjectStackLayout.BindingContext = NewSubjectViewModel;
        }
    }
}