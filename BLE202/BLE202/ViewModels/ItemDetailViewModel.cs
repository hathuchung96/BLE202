using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using BLE202.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BLE202.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        #region Khai báo
        private string itemId;
        private string itemservice;
        private string text;
        private string messagex;
        private string dataa;
        private string itemcharacteristic;
        private ICharacteristic charec;
        public string Id { get; set; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        public string Messagex
        {
            get => messagex;
            set => SetProperty(ref messagex, value);
        }
        public string Characteristic
        {
            get => itemcharacteristic;
            set => SetProperty(ref itemcharacteristic, value);
        }
        public string DataSend
        {
            get => dataa;
            set => SetProperty(ref dataa, value);
        }
        public string Servicex
        {
            get => itemservice;
            set => SetProperty(ref itemservice, value);
        }
        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }
        public ICharacteristic Charec
        {
            get => charec;
            set => SetProperty(ref charec, value);
        }
        public ICharacteristic Statics;
        #endregion
        public ItemDetailViewModel()
        {
            Title = "Device Information";
            OpenWebCommand = new Command(async () =>
            {
                if (Messagex != null)
                {
                    var data = Encoding.ASCII.GetBytes(Messagex);
                    await Charec.WriteAsync(data);
                    DataSend += "[Done]Send Message : " + Messagex + " \r\n";
                }
                else DataSend += "[Error]Plese input message \r\n";
            });
            /*
            var minutes = TimeSpan.FromSeconds(2);

            Device.StartTimer(minutes, async () =>
            {

                var bytes = await Charec.ReadAsync();
                string result = System.Text.Encoding.UTF8.GetString(bytes);
                DataSend += "[Respone]Get dataaaa \r\n";
                return true;
            });
            */
        }

        public ICommand OpenWebCommand { get; }
        public async void LoadItemId(string itemId)
        {
            try
            {
                Debug.WriteLine(itemId);
                var item = await DataStore.GetItemAsync(itemId);
                
                Id = item.Id;
                Text = "Device Info: "+ item.Text;
                        var adapter = CrossBluetoothLE.Current.Adapter;
                        UserDialogs.Instance.Toast("Try connect to device, please đợi 1 chút !");

                        try
                        {
                            await adapter.ConnectToDeviceAsync(item.Device);
                            Acr.UserDialogs.UserDialogs.Instance.Alert("Kết nối thành công, ahihi!", "Ok");
                            UserDialogs.Instance.Toast("Đang mò gởi data, please đợi 1 chút !");
                            var services = await item.Device.GetServicesAsync();
                            // Get Only Service type Unknown Service.
                            for (int i = 0; i < services.Count(); i++)
                                if (services[i].Name.ToLower().CompareTo("unknown service") == 0)
                                {
                                    var characteristics = await services[i].GetCharacteristicsAsync();
                                    
                                    for (int j = 0; j < characteristics.Count(); j++)
                                    {
                                if (characteristics[j].CanWrite == true && characteristics[j].CanUpdate == true && characteristics[j].CanRead == true)
                                {
                                            Servicex = "Service: " + services[i].Id.ToString();
                                            Characteristic = characteristics[j].Id.ToString();
                                            Charec = characteristics[j];
                                            var data = Encoding.ASCII.GetBytes("Hello BLE server !!!");                                           
                                            await characteristics[j].WriteAsync(data);
                                            DataSend += "[Done]Send Message : Hello BLE server !!! \r\n";
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

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
