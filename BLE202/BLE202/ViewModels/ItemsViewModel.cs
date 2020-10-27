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

        public ItemsViewModel()
        {
            Title = "BLE Devices";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<Item>(OnItemSelected);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            UserDialogs.Instance.Toast("Scanning Devices BLE, Please waits.");

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
                UserDialogs.Instance.Toast("Scanning Devices BLE, Please wait 5s.");

                Items.Clear();
                var ble = CrossBluetoothLE.Current;
                var adapter = CrossBluetoothLE.Current.Adapter;
                var state = ble.State;

                adapter.DeviceDiscovered += (s, a) => {
                    Item b = new Item
                    {
                        Text = (a.Device.Name!=null ? a.Device.Name : "Unknown")+ " | MAC: " + a.Device.NativeDevice.ToString(),
                        Description =a.Device.Rssi.ToString() + " dBm",
                        Id = a.Device.Id.ToString(),
                        Device = a.Device
                    };
                    if (!Items.Any(x => x.Id == b.Id) && a.Device.Name != null)
                    {
                        Items.Add(b);
                        DataStore.AddItemAsync(b);
                    }
                }; 
                adapter.ScanTimeout = 5000;
                adapter.ScanMode = ScanMode.LowLatency;
                await adapter.StartScanningForDevicesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
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
            //           await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
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
                    if (item.Device!= null)
                    await Clipboard.SetTextAsync(item.Device.Id.ToString());
                    UserDialogs.Instance.Toast("Copy GUID device done !");
                });
                config.Cancel = new ActionSheetOption("Cancel");
                config.SetTitle("Device Options");
                UserDialogs.Instance.ActionSheet(config);

            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}