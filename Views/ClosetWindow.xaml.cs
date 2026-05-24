using Microsoft.Win32;
using Modesta.Models;
using Modesta.Services;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Modesta.Views
{
    public partial class ClosetWindow : Window
    {
        private User _currentUser;
        private ClosetService _closetService = new ClosetService();

        public ClosetWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadCollections();
            LoadItems();
        }

        private void LoadCollections()
        {
            var collections = _closetService.GetCollections(_currentUser.UserId);
            CollectionCombo.ItemsSource = collections;
            CollectionCombo.DisplayMemberPath = "Name";
            CollectionCombo.SelectedValuePath = "CollectionId";
        }

        private void LoadItems(int? collectionId = null)
        {
            ItemsPanel.Children.Clear();
            var items = collectionId.HasValue
                ? _closetService.GetItems(_currentUser.UserId)
                    .Where(i => i.CollectionId == collectionId.Value).ToList()
                : _closetService.GetItems(_currentUser.UserId);

            foreach (var item in items)
            {
                var card = CreateItemCard(item);
                ItemsPanel.Children.Add(card);
            }
        }

        private Border CreateItemCard(ClothingItem item)
        {
            var border = new Border
            {
                Width = 160,
                Height = 200,
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#E0D5C5")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(8)
            };

            var stack = new StackPanel();

            if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
            {
                var img = new System.Windows.Controls.Image
                {
                    Height = 120,
                    Stretch = System.Windows.Media.Stretch.Uniform
                };
                img.Source = new BitmapImage(new System.Uri(item.PhotoPath));
                stack.Children.Add(img);
            }
            else
            {
                var placeholder = new Border
                {
                    Height = 120,
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#E8DDD0"))
                };
                stack.Children.Add(placeholder);
            }

            stack.Children.Add(new TextBlock
            {
                Text = item.Name,
                FontWeight = FontWeights.Medium,
                FontSize = 12,
                Margin = new Thickness(8, 6, 8, 2),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#3A2E22"))
            });

            stack.Children.Add(new TextBlock
            {
                Text = item.Category,
                FontSize = 10,
                Margin = new Thickness(8, 0, 8, 4),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#7A6B5A"))
            });

            border.Child = stack;
            return border;
        }

        private void CollectionCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionCombo.SelectedValue != null)
                LoadItems((int)CollectionCombo.SelectedValue);
            else
                LoadItems();
        }

        private void AddCollection_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewCollectionBox.Text)) return;
            _closetService.CreateCollection(_currentUser.UserId, NewCollectionBox.Text);
            NewCollectionBox.Text = "";
            LoadCollections();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (CollectionCombo.SelectedValue == null)
            {
                MessageBox.Show("Kies eerst een collectie.", "Fout");
                return;
            }

            var dialog = new OpenFileDialog();
            dialog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png";
            if (dialog.ShowDialog() == true)
            {
                string folder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Photos");
                Directory.CreateDirectory(folder);
                string filename = Guid.NewGuid()
                    + Path.GetExtension(dialog.FileName);
                string dest = Path.Combine(folder, filename);
                File.Copy(dialog.FileName, dest);

                var nameDialog = Microsoft.VisualBasic.Interaction.InputBox(
                    "Naam van het kledingstuk:", "Item toevoegen", "");
                var category = Microsoft.VisualBasic.Interaction.InputBox(
                    "Categorie (bv. Kleding, Schoenen, Tas):", "Categorie", "");

                _closetService.AddItem(
                    _currentUser.UserId,
                    (int)CollectionCombo.SelectedValue,
                    nameDialog, category, "", "", dest, 0);

                LoadItems();
            }
        }
    }
}