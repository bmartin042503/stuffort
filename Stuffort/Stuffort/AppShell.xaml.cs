using System;
using Stuffort.Configuration;
using Stuffort.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffort
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public MainViewModel MainViewModel;
        public ConfigurationType ConfType;
        public AppShell()
        {
            InitializeComponent();
        }
    }
}