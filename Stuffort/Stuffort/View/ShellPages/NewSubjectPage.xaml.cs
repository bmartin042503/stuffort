﻿using Stuffort.Resources;
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
    public partial class NewSubjectPage : ContentPage
    {
        public NewSubjectViewModel NewSubjectViewModel;
        public NewSubjectPage()
        {
            InitializeComponent();
            this.NewSubjectViewModel = new NewSubjectViewModel();
            BindingContext = NewSubjectViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CurrentTitle.Text = AppResources.NewSubjectPage;
            this.Title = AppResources.NewSubjectPage;
            subjectNameEntry.Placeholder = AppResources.Name;
            saveBtn.Text = AppResources.Save;
        }
    }
}