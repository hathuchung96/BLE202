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

            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            UserDialogs.Instance.Toast("Scanning Devices BLE, Please wait 1 chút.");

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
                            UserDialogs.Instance.Toast("Aw, you không cho quyền thì quét cái con chim!");
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

        private async void OnAddItem(object obj)
        {
//            await Shell.Current.GoToAsync(nameof(NewItemPage));
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

                    await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
                    /* 
                     var adapter = CrossBluetoothLE.Current.Adapter;
                     UserDialogs.Instance.Toast("Try connect to device, please đợi 1 chút !");

                     try
                     {
                         await adapter.ConnectToDeviceAsync(item.Device);
                         Acr.UserDialogs.UserDialogs.Instance.Alert("Kết nối thành công, ahihi!", "Ok");

                         var services = await item.Device.GetServicesAsync();

                         string t = "";
                         // Get Only Service type Unknown Service.
                         for (int i=0; i < services.Count(); i++)
                             if (services[i].Name.ToLower().CompareTo("unknown service") == 0)
                             {
                                 var characteristics = await services[i].GetCharacteristicsAsync();



                                 for (int j=0; j < characteristics.Count(); j++)
                                 {
                                    // t+= characteristics[j].WriteType.ToString()+":"+ characteristics[j].CanRead
                                    if (characteristics[j].CanWrite == true && characteristics[j].CanUpdate == true && characteristics[j].CanRead == true)
                                     {
                                         t += "Connect to Service ID: \r\n" + services[i].Id.ToString() + "\r\n Characteristics ID : \r\n";
                                         t += characteristics[j].Id.ToString() + "\r\n Send String 'Hello BLE server'";
                                         var data = Encoding.ASCII.GetBytes("Hello BLE server !!!");
                                         await characteristics[j].WriteAsync(data);
                                         Acr.UserDialogs.UserDialogs.Instance.Alert(t, "Ok");
                                     }
                                 }


                             }
                         //       UserDialogs.Instance.Toast(characteristics[0]..ToString() +"\r\n dd");
                     }
                     catch (DeviceConnectionException ex)
                     {
                         UserDialogs.Instance.Toast("Lỗi rồi ahuhu !");
                     }
                     catch (Exception ex)
                     {
                         UserDialogs.Instance.Toast("Lỗi rồi ahuhu !");
                     }   

                     */
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