using System;
using System.Collections.Generic;
using System.Text;
using Stuffort.View;
using Stuffort.ViewModel.Commands;

namespace Stuffort.ViewModel
{
    public class MainViewModel
    {
        public List<string> Languages { get; set; }
        public MainPageCommand MainPageCommand { get; set; }
        public MainViewModel()
        {
            this.MainPageCommand = new MainPageCommand(this);
            Languages = new List<string>()
            {
                "English", "Magyar", "Polski"
            };
        }

        public async void NavigateToHomepage()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new HomePage());
        }
    }
}
