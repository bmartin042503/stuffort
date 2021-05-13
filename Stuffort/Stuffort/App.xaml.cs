using Stuffort.Resources;
using Stuffort.View;
using Stuffort.View.ShellPages;
using Stuffort.Configuration;
using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

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
            //File.Delete(ConfigurationServices.FilePath);
            //File.Delete(location);
            //StatisticsServices.DeleteAll();
            DatabaseLocation = location;
            Xamarin.Essentials.VersionTracking.Track();
            MainPage = new AppShell();
            ConfigurationType ct = ConfigurationServices.GetConfigurationData();
            if (ct.Language != "undefined")
            {
                CultureInfo language = new CultureInfo(ct.Language);
                Thread.CurrentThread.CurrentUICulture = language;
                AppResources.Culture = language;
            }
            else
            {
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                CultureInfo lang = new CultureInfo(language.Substring(0, 2));
                AppResources.Culture = lang;
            }
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
