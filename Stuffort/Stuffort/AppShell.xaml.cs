using System;
using Stuffort.Configuration;
using Stuffort.ViewModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Stuffort.View.ShellPages;

namespace Stuffort
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(nameof(NewSubjectPage), typeof(NewSubjectPage));
            Routing.RegisterRoute(nameof(NewTaskPage), typeof(NewTaskPage));
        }
    }
}