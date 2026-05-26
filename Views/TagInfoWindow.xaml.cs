using Modesta.Models;
using Modesta.Services;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;

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

            if (_tag.Post == null)
            {
                using (var db = new Modesta.Data.ModestDbContext())
                {
                    var fullTag = db.ItemTags
                        .Include(t => t.Post)
                        .FirstOrDefault(t => t.TagId == _tag.TagId);
                    if (fullTag != null) _tag = fullTag;
                }
            }

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

            // Zorg dat "Opgeslagen items" bestaat
            _closetService.GetOrCreateSavedCollection(_currentUser.UserId);

            var collections = _closetService.GetCollections(_currentUser.UserId);
            CollectionCombo.ItemsSource = collections;
            CollectionCombo.DisplayMemberPath = "Name";
            CollectionCombo.SelectedValuePath = "CollectionId";

            // Selecteer standaard "Opgeslagen items"
            var savedCol = collections.FirstOrDefault(c => c.Name == "Opgeslagen items");
            if (savedCol != null)
                CollectionCombo.SelectedValue = savedCol.CollectionId;
            else
                CollectionCombo.SelectedIndex = 0;
        }

        private void Link_Click(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(_tag.OnlineLink))
                Process.Start(new ProcessStartInfo(_tag.OnlineLink)
                { UseShellExecute = true });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var collectionId = CollectionCombo.SelectedValue;
            if (collectionId == null)
            {
                var saved = _closetService.GetOrCreateSavedCollection(_currentUser.UserId);
                collectionId = saved.CollectionId;
            }

            string photoPath = _tag.ProductPhotoPath ?? "";
            if (string.IsNullOrEmpty(photoPath) && _tag.Post != null &&
                !string.IsNullOrEmpty(_tag.Post.ImageUrl))
            {
                photoPath = _tag.Post.ImageUrl;
            }

            _closetService.AddItem(
                _currentUser.UserId,
                (int)collectionId,
                _tag.Brand ?? _tag.ItemType,
                _tag.ItemType,
                "", _tag.Brand ?? "",
                photoPath, 0);

            MessageBox.Show("Item opgeslagen in je kledingkast!", "Gelukt");
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}