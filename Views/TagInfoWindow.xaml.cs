using Modesta.Models;
using Modesta.Services;
using System.Diagnostics;
using System.Windows;

namespace Modesta.Views
{
    public partial class TagInfoWindow : Window
    {
        private ItemTag _tag;
        private User _currentUser;
        private ClosetService _closetService = new ClosetService();

        public TagInfoWindow(ItemTag tag, User currentUser)
        {
            InitializeComponent();
            _tag = tag;
            _currentUser = currentUser;
            LoadInfo();
        }

        private void LoadInfo()
        {
            TitleText.Text = $"{_tag.ItemType} — {_tag.Brand}";
            TypeText.Text = $"Type: {_tag.ItemType}";
            BrandText.Text = $"Merk: {_tag.Brand}";

            if (!string.IsNullOrEmpty(_tag.ShopName))
                ShopText.Text = $"Winkel: {_tag.ShopName}";
            else
                ShopText.Visibility = Visibility.Collapsed;

            if (!string.IsNullOrEmpty(_tag.ShopAddress))
                AddressText.Text = $"Adres: {_tag.ShopAddress}";
            else
                AddressText.Visibility = Visibility.Collapsed;

            if (!string.IsNullOrEmpty(_tag.OnlineLink))
                LinkText.Text = _tag.OnlineLink;
            else
                LinkText.Visibility = Visibility.Collapsed;
        }

        private void Link_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(_tag.OnlineLink))
                Process.Start(new ProcessStartInfo(_tag.OnlineLink)
                { UseShellExecute = true });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var collections = _closetService.GetCollections(_currentUser.UserId);
            if (collections.Count == 0)
            {
                MessageBox.Show("Maak eerst een collectie aan in je kledingkast.", "Fout");
                return;
            }
            _closetService.AddItem(
                _currentUser.UserId,
                collections[0].CollectionId,
                _tag.Brand ?? _tag.ItemType,
                _tag.ItemType,
                "", _tag.Brand ?? "",
                _tag.ProductPhotoPath ?? "", 0);
            MessageBox.Show("Item opgeslagen in je kledingkast!", "Gelukt");
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}