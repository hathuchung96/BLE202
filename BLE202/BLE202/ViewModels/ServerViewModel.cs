using Acr.UserDialogs;
using nexus.core;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using nexus.protocols.ble.scan;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BLE202.ViewModels
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private string buttonstr;
        public string ButtonStr
        {
            get { return buttonstr; }
            set
            {
                buttonstr = value;
                OnPropertyChanged();
            }
        }
        private Boolean m_isBusy;
        public Boolean IsBusy
        {
            get { return m_isBusy; }
            set
            {
                m_isBusy = value;
                OnPropertyChanged();
            }
        }

        private string m_connectionState;
        public string Connection
        {
            get { return m_connectionState; }
            set
            {
                m_connectionState = value;
                OnPropertyChanged();
            }
        }

        private string logstr;
        public string LogStr
        {
            get { return logstr; }
            set
            {
                logstr = value;
                OnPropertyChanged();
            }
        }

        public ServerViewModel()
        {
            //           Title = "Server BLE";

            ButtonStr = "Start Server";
            OpenWebCommand = new Command(async () => 
            {
                if (ButtonStr == "Start Server")
                {
                    ButtonStr = "Stop Server";
                    LogStr += "Start Server \r\n";
                  



                }
                else 
                {
                    ButtonStr = "Start Server";
                    LogStr += "Stop Server \r\n";
                }
            });
            // Acr.UserDialogs.UserDialogs.Instance.Alert("This page not hoàn thành yet, quay lại sau nhé :)", "Ok");


        }    

        public ICommand OpenWebCommand { get; }
   

    }
}