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
        private string itemwritecharacteristic;
        private string itemreadcharacteristic;
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
        public string WriteCharacteristic
        {
            get => itemwritecharacteristic;
            set => SetProperty(ref itemwritecharacteristic, value);
        }
        public string ReadCharacteristic
        {
            get => itemreadcharacteristic;
            set => SetProperty(ref itemreadcharacteristic, value);
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
                    try
                    {
                        var data = Encoding.ASCII.GetBytes(Messagex);
                        await Charec.WriteAsync(data);
                        DataSend += "[Write Data] " + Messagex + " \r\n";
                    }
                    catch (Exception ex)
                    {
                        DataSend += "[Error]Can not send message \r\n";
                    }
                }
                else DataSend += "[Error]Plese input message \r\n";
            });
        }

        public ICommand OpenWebCommand { get; }
        public async void LoadItemId(string itemId)
        {
            try
            {
                Debug.WriteLine(itemId);
                var item = await DataStore.GetItemAsync(itemId);

                Id = item.Id;
                Text =item.AddressAndName;
                bool checkswrite = false;
                bool checksread = false;
                try
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Connect Success!", "Ok");
                    DataSend += "Try to discover services...\r\n";
                    // Get Only Service type Unknown Service.
                    foreach (var service in await item.Device.GetServicesAsync())
                    {
                        foreach (var characteristic in await service.GetCharacteristicsAsync())
                        {
                            if (characteristic.CanWrite && characteristic.CanUpdate && characteristic.CanRead)
                            {
                                if (!checkswrite)
                                {
                                    Servicex = service.Id.ToString();
                                    WriteCharacteristic = characteristic.Id.ToString();
                                    Charec = characteristic;
                                    var data = Encoding.ASCII.GetBytes("Hello Server ~~~");
                                    await characteristic.WriteAsync(data);
                                    DataSend += "[Write Data] Hello Server ~~~\r\n";
                                    checkswrite = true;
                                } else if (!checksread)
                                {
                                    ReadCharacteristic = characteristic.Id.ToString();
                                    var cagsxc = characteristic;
                                    Task taskA = Task.Run(async () =>
                                    {
                                        cagsxc.ValueUpdated += (o, args) =>
                                        {
                                            var bytes = args.Characteristic.Value;
                                            string result = System.Text.Encoding.UTF8.GetString(bytes);
                                            DataSend += "[Read Data] " + result + " \r\n";
                                        };

                                    });

                                    checksread = true;
                                }

                            }
                        }
                    }
                }
                catch (DeviceConnectionException ex)
                {
                    UserDialogs.Instance.Toast("Error, please try again.");
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Toast("Error, please try again.");
                }

            }
            catch (Exception)
            {
                UserDialogs.Instance.Toast("Error, please try again.");
            }
        }
    }
}
