using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stuffort
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel MainViewModel;
        public MainPage()
        {
            InitializeComponent();
            string language = Thread.CurrentThread.CurrentUICulture.Name;
            languagePicker.SelectedIndex = language == "pl" ? 2 : language == "hu" ? 1 : 0;
            MainViewModel = new MainViewModel();
            languageSelectionStackLayout.BindingContext = MainViewModel;
        }

        private void languagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainViewModel.MainPageCommand.Execute(sender as Picker);
        }
    }
}
