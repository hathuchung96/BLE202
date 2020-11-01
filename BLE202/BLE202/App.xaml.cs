using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BLE202.Services;
using BLE202.Views;
using nexus.protocols.ble;
using Acr.UserDialogs;
using BLE202.ViewModels;

namespace BLE202
{
    public partial class App : Application
    {
        public App(IBluetoothLowEnergyAdapter adapter, IUserDialogs dialogs)
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();

            MessagingCenter.Subscribe<App, string>((App)global::Xamarin.Forms.Application.Current, "IBluetoothLowEnergyAdapterX", async (sender, arg) =>
            {
                MessagingCenter.Send<BLE202.App, IBluetoothLowEnergyAdapter>((BLE202.App)Xamarin.Forms.Application.Current, "SendAdppter", adapter);
            });
            

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
