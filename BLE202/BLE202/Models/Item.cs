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

    }
}