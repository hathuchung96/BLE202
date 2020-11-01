using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using Plugin.BLE.Abstractions.Contracts;
using System;

namespace BLE202.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public IDevice Device { get; set; }
        public IBleGattServerConnection Server { get; set; }


        public string AddressAndName { get; set; }
        public string RSSITx { get; set; }
        public string Flags { get; set; }

        public string Mfg { get; set; }
    }
}