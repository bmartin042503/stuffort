﻿using Stuffort.Configuration;
using Stuffort.Resources;
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
        public SettingsViewModel SettingsViewModel;
        public SettingsPage()
        {
            InitializeComponent();
            SettingsViewModel = new SettingsViewModel(ConfigurationServices.GetConfigurationData(), languagePicker);
            languagePicker.ItemsSource = new List<string>()
            {
                "English", "Magyar", "Polski"
            };
            languagePicker.SelectedIndex = 0;
            BindingContext = this.SettingsViewModel;
            languageIconImage.Source = ImageSource.FromResource("Stuffort.Resources.Images.ic_translate.png");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CurrentTitle.Text = AppResources.ResourceManager.GetString("SettingsPage");
            saveBtn.Text = AppResources.ResourceManager.GetString("Save");
            languageLbl.Text = AppResources.ResourceManager.GetString("Language");
        }
    }
}