using System;
using System.Collections.Generic;
using BLE202.ViewModels;
using BLE202.Views;
using Xamarin.Forms;

namespace BLE202
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));

        }

    }
}
