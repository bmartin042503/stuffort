﻿using System;
using Stuffort.Configuration;
using Stuffort.ViewModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffort
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
    }
}