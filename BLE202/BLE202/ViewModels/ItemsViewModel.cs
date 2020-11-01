using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;

using BLE202.Models;
using BLE202.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading;
using Xamarin.Essentials;
using Plugin.BLE.Abstractions.Exceptions;
using System.Linq;
using Acr.Collections;
using System.Text;
using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using nexus.core.text;
using nexus.core;
using nexus.protocols.ble.scan.advertisement;

namespace BLE202.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;
        private IUserDialogs _userDialogs;
        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }
        private IBluetoothLowEnergyAdapter t;
        public IBluetoothLowEnergyAdapter Ta
        {
            get { return t; }
            set { t = value; }
        }
        public ItemsViewModel()
        {

            Title = "BLE Devices";

            MessagingCenter.Subscribe<App, IBluetoothLowEnergyAdapter>((App)global::Xamarin.Forms.Application.Current, "SendAdppter", async (sender, arg) =>
            {
                Ta = arg;
            });

            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<Item>(OnItemSelected);

            MessagingCenter.Send<BLE202.App, string>((BLE202.App)Xamarin.Forms.Application.Current, "IBluetoothLowEnergyAdapterX", "");
        }
        async Task ExecuteLoadItemsCommand()
        {

            IsBusy = true;
            UserDialogs.Instance.Toast("Scanning Devices BLE, Please waits.");
            if (Ta != null)
                try
                {
                    if (Xamarin.Forms.Device.RuntimePlatform == Device.Android)
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                        if (status != PermissionStatus.Granted)
                        {
                            var permissionResult = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                            if (permissionResult != PermissionStatus.Granted)
                            {
                                await _userDialogs.AlertAsync("Permission denied. Not scanning.");
                                UserDialogs.Instance.Toast("Aw, You don't have permission for this app.");
                                return;
                            }
                        }
                    }

                    Items.Clear();
                  /*  var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3000));
                    cts.CancelAfter(3000);
                    await Ta.ScanForBroadcasts(
                       // providing ScanSettings is optional
                       new ScanSettings() { Mode = nexus.protocols.ble.scan.ScanMode.LowPower },
                       (IBlePeripheral peripheral) =>
                       {
                           var adv = peripheral.Advertisement;
                           string MAC = peripheral.Address.Select(bc => bc.EncodeToBase16String()).Join(":");
                           Item b = new Item
                           {
                               AddressAndName = MAC + " / " + (adv.DeviceName != null ? adv.DeviceName : "Unknown"),
                               RSSITx = peripheral.Rssi.ToString() + " / " + peripheral.Advertisement.TxPowerLevel.ToString(),
                               Flags = peripheral.Advertisement?.Flags.ToString("G"),
                               Mfg = peripheral.Advertisement.ManufacturerSpecificData
                                             .Select(
                                                x => x.CompanyName() + "=0x" +
                                                     x.Data?.ToArray()?.EncodeToBase16String()).Join(", ")
                           };
                           if (!Items.Any(x => x.Id == b.Id) && adv.DeviceName != null)
                           {
                               Items.Add(b);
                               DataStore.AddItemAsync(b);
                           }
                       }, cts.Token
                       );*/
                    var ble = CrossBluetoothLE.Current;
                    var adapter = CrossBluetoothLE.Current.Adapter;
                    var state = ble.State;

                    adapter.DeviceDiscovered += async (s, a) => {
                        Item b = new Item
                        {
                            Text = a.Device.Name != null ? a.Device.Name : "Unknown",
                            Description = a.Device.Rssi.ToString() + " dBm",
                            Id = a.Device.Id.ToString(),
                            AddressAndName = a.Device.NativeDevice.ToString() + " / " + (a.Device.Name != null ? a.Device.Name : "Unknown"),
                            Device = a.Device
                        };
                        if (!Items.Any(x => x.Id == b.Id) && a.Device.Name != null)
                        {
                            Items.Add(b);
                            await DataStore.AddItemAsync(b);
                        } else if (a.Device.Name != null)
                        {
                            Item c = await DataStore.GetItemAsync(b.Id);
                            c.Text = b.Text;
                            c.Description = b.Description;
                            c.AddressAndName = b.AddressAndName;
                            c.Device = b.Device;
                            await DataStore.UpdateItemAsync(c);
                        }
                    };
                    adapter.ScanTimeout = 3000;
                    adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.LowLatency;
                    await adapter.StartScanningForDevicesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    IsBusy = false;

                    UserDialogs.Instance.Toast("Scanning Devices BLE Done");
                }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        async void OnItemSelected(Item item)
        {

            if (item == null)
                return;
            try
            {
                var config = new ActionSheetConfig();

                config.Add("Connect", async () =>
                {
                    var adapter = CrossBluetoothLE.Current.Adapter;
                    UserDialogs.Instance.Toast("Try connect to device, please wait !");
                    await adapter.ConnectToDeviceAsync(item.Device);
                    await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
                });
                config.Add("Copy GUID", async () =>
                {
                    if (item.Device != null)
                        await Clipboard.SetTextAsync(item.Device.Id.ToString());
                    UserDialogs.Instance.Toast("Copy GUID device done !");
                });
                config.Cancel = new ActionSheetOption("Cancel");
                config.SetTitle("Device Options");
                UserDialogs.Instance.ActionSheet(config);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }


}