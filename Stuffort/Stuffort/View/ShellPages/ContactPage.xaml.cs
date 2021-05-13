using Stuffort.Resources;
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
    public partial class ContactPage : ContentPage
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Title = AppResources.ContactPage;
            labelVersion.Text = $"{AppResources.Version} {VersionTracking.CurrentBuild} ({VersionTracking.CurrentVersion})";
            if (VersionTracking.IsFirstLaunchForCurrentBuild)
                labelVersion.Text = $"{AppResources.NewVersion}: {labelVersion.Text}";
            allRightsReserved.Text = AppResources.AllRightsReserved;
            labelDeveloper.Text = AppResources.Developer;
        }
    }
}