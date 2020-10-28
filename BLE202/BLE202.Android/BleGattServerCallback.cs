using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace BLE202.Droid
{
    public class BleEventArgs : EventArgs
    {
        public BluetoothDevice Device { get; set; }
        public GattStatus GattStatus { get; set; }
        public BluetoothGattCharacteristic Characteristic { get; set; }
        public byte[] Value { get; set; }
        public int RequestId { get; set; }
        public int Offset { get; set; }

        public BluetoothGattDescriptor Descriptor { get; set; }

        public bool PreparedWrite { get; set; }

    }

    public class BleGattServerCallback : BluetoothGattServerCallback
    {

        public event EventHandler<BleEventArgs> NotificationSent;
        public event EventHandler<BleEventArgs> CharacteristicReadRequest;
        public event EventHandler<BleEventArgs> DescriptorWriteRequest;
        public event EventHandler<BleEventArgs> CharacteristicWriteRequest;
        public BleGattServerCallback()
        {

        }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);
            MessagingCenter.Send<BLE202.App, string>((BLE202.App)Xamarin.Forms.Application.Current, "Hi", "Read request from "+ device.Name);

            if (CharacteristicReadRequest != null)
            {
                CharacteristicReadRequest(this, new BleEventArgs() { Device = device, Characteristic = characteristic, RequestId = requestId, Offset = offset });
            }
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);

            if (CharacteristicWriteRequest != null)
            {
                CharacteristicWriteRequest(this, new BleEventArgs() { Device = device, RequestId = requestId, Characteristic = characteristic, PreparedWrite = preparedWrite, Offset = offset, Value = value });
            }
           
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            MessagingCenter.Send<BLE202.App, string>((BLE202.App)Xamarin.Forms.Application.Current, "Hi", "["+ newState.ToString()+ " Device]Mac:  "+ device.Address.ToString());
            // CharacteristicWriteRequest(this, new BleEventArgs() { Device = device });
            //  Console.WriteLine("State changed to {0}", newState);

        }
        public override void OnDescriptorReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattDescriptor descriptor)
        {
            base.OnDescriptorReadRequest(device, requestId, offset, descriptor);
            MessagingCenter.Send<BLE202.App, string>((BLE202.App)Xamarin.Forms.Application.Current, "Hi", "read Value descr: ");
        }
        public override void OnDescriptorWriteRequest(BluetoothDevice device, int requestId, BluetoothGattDescriptor descriptor, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnDescriptorWriteRequest(device, requestId, descriptor, preparedWrite, responseNeeded, offset, value);


            if (DescriptorWriteRequest != null)
            {
                DescriptorWriteRequest(this, new BleEventArgs() { Device = device, RequestId = requestId, Descriptor = descriptor, PreparedWrite = preparedWrite, Offset = offset, Value = value });
            }
        }

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            base.OnNotificationSent(device, status);
            MessagingCenter.Send<BLE202.App, string>((BLE202.App)Xamarin.Forms.Application.Current, "Hi", "wrinotify te Value descr: ");
          
        }

    }
}