﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzU1Mjk3QDMxMzgyZTMzMmUzMGloVERVWXNDOTdjSUF2UU91TWk4b3R1TUQ5YUI0bXhEcVRGYXJDQjRhYWM9");

            MainPage = new NavigationPage(new MainPage());
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
