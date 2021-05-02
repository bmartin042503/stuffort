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
            labelVersion.Text = $"{AppResources.ResourceManager.GetString("Version")} {VersionTracking.CurrentBuild} ({VersionTracking.CurrentVersion})";
        }
    }
}