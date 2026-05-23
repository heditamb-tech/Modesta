using Microsoft.Win32;
using Modesta.Models;
using Modesta.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Modesta.Views
{
    public partial class PostWindow : Window
    {
        private User _currentUser;
        private PostService _postService = new PostService();
        private string _imagePath;
        private List<ItemTag> _tags = new List<ItemTag>();

        public PostWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void PhotoBorder_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png";
            if (dialog.ShowDialog() == true)
            {
                string folder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Photos");
                Directory.CreateDirectory(folder);
                string filename = Guid.NewGuid() + Path.GetExtension(dialog.FileName);
                string dest = Path.Combine(folder, filename);
                File.Copy(dialog.FileName, dest);
                _imagePath = dest;
                PreviewImage.Source = new BitmapImage(new Uri(dest));
                PreviewImage.Visibility = Visibility.Visible;
                PhotoBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            var itemType = Microsoft.VisualBasic.Interaction.InputBox(
                "Type item (bv. Kleding, Schoenen, Tas, Hijab):", "Item type", "");
            var brand = Microsoft.VisualBasic.Interaction.InputBox(
                "Merk:", "Merk", "");
            var onlineLink = Microsoft.VisualBasic.Interaction.InputBox(
                "Online link (leeg laten indien niet van toepassing):", "Link", "");
            var shopName = Microsoft.VisualBasic.Interaction.InputBox(
                "Winkelnaam (leeg laten indien niet van toepassing):", "Winkel", "");
            var shopAddress = Microsoft.VisualBasic.Interaction.InputBox(
                "Winkeladres (leeg laten indien niet van toepassing):", "Adres", "");

            if (!string.IsNullOrEmpty(itemType))
            {
                var tag = new ItemTag
                {
                    ItemType = itemType,
                    Brand = brand,
                    OnlineLink = onlineLink,
                    ShopName = shopName,
                    ShopAddress = shopAddress
                };
                _tags.Add(tag);

                var tb = new System.Windows.Controls.TextBlock
                {
                    Text = $"✓ {itemType} — {brand}",
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#5C4A32")),
                    Margin = new Thickness(0, 0, 0, 4)
                };
                TagsList.Items.Add(tb);
            }
        }

        private void Publish_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_imagePath))
            {
                MessageBox.Show("Upload eerst een foto.", "Fout");
                return;
            }
            if (_tags.Count == 0)
            {
                MessageBox.Show("Voeg minstens één item tag toe.", "Fout");
                return;
            }

            var post = _postService.CreatePost(
                _currentUser.UserId, _imagePath,
                CaptionBox.Text, "public");

            foreach (var tag in _tags)
            {
                _postService.AddTag(post.PostId, tag.ItemType, tag.Brand,
                                    tag.OnlineLink, tag.ShopName, tag.ShopAddress);
            }

            MessageBox.Show("Post gepubliceerd!", "Gelukt");
            this.Close();
        }
    }
}