using Stuffort.Resources;
using Stuffort.View;
using Stuffort.View.ShellPages;
using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace Stuffort
{
    public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;
        public App()
        {
            CultureInfo language = new CultureInfo("pl");
            AppResources.Culture = language;
            InitializeComponent();

            MainPage = new AppShell();
        }

        public App(string location)
        {
            InitializeComponent();
            DatabaseLocation = location;
            Xamarin.Essentials.VersionTracking.Track();
            MainPage = new AppShell();
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
