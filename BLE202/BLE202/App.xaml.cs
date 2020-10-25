﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BLE202.Services;
using BLE202.Views;

namespace BLE202
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
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
