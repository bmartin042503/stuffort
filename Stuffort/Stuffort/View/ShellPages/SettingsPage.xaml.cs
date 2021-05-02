using Stuffort.Configuration;
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
    public partial class SettingsPage : ContentPage
    {
        public MainViewModel MainViewModel;
        public SettingsPage()
        {
            InitializeComponent();
            var name = this.GetType().Name;
            this.MainViewModel = new MainViewModel(ConfigurationServices.GetConfigurationData(), languagePicker, name);
            BindingContext = MainViewModel;
            var assembly = typeof(SettingsPage);
            languageIconImage.Source = ImageSource.FromResource("Stuffort.Resources.Images.language.jpg");
        }
    }
}