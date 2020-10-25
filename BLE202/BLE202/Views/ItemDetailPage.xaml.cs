using System.ComponentModel;
using Xamarin.Forms;
using BLE202.ViewModels;

namespace BLE202.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}