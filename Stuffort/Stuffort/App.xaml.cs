using Stuffort.View;
using Stuffort.View.ShellPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffort
{
    public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        public App(string location)
        {
            InitializeComponent();
            DatabaseLocation = location;
            MainPage = new AppShell();
            Routing.RegisterRoute(nameof(NewSubjectPage), typeof(NewSubjectPage));
            Routing.RegisterRoute(nameof(NewTaskPage), typeof(NewTaskPage));
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
