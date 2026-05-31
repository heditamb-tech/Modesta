using Microsoft.Win32;
using Modesta.Models;
using Modesta.Services;
using System;
using System.IO;
using System.Linq;
using System.Windows;
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
            _closetService.GetOrCreateSavedCollection(_currentUser.UserId);
            _closetService.GetOrCreateMyItemsCollection(_currentUser.UserId);
            _closetService.GetOrCreateOutfitBuilderCollection(_currentUser.UserId);
            LoadCollections();
            ItemsPanel.Children.Clear();
            var hint = new System.Windows.Controls.TextBlock
            {
                Text = "Kies een collectie om je items te zien.",
                FontSize = 13,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#7A6B5A")),
                Margin = new System.Windows.Thickness(8, 16, 0, 0)
            };
            ItemsPanel.Children.Add(hint);
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
                Height = 240,
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
                img.Source = new BitmapImage(new Uri(item.PhotoPath));
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
                Margin = new Thickness(8, 0, 8, 6),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#7A6B5A"))
            });

            var btnPanel1 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(8, 0, 8, 4)
            };

            var editBtn = new Button
            {
                Content = "Bewerk",
                FontSize = 10,
                Height = 28,
                Padding = new Thickness(8, 0, 8, 0),
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#F5EFE6")),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")),
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#E0D5C5")),
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 4, 0)
            };

            var capturedItem = item;
            editBtn.Click += (s, e) =>
            {
                var newName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Nieuwe naam:", "Bewerk item", capturedItem.Name);
                var newCategory = Microsoft.VisualBasic.Interaction.InputBox(
                    "Nieuwe categorie:", "Bewerk item", capturedItem.Category);
                if (!string.IsNullOrEmpty(newName))
                {
                    _closetService.UpdateItem(capturedItem.ItemId, newName, newCategory);
                    LoadItems();
                }
            };

            var deleteBtn = new Button
            {
                Content = "Verwijder",
                FontSize = 10,
                Height = 28,
                Padding = new Thickness(8, 0, 8, 0),
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#FAECE7")),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#712B13")),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            deleteBtn.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    $"'{capturedItem.Name}' verwijderen?",
                    "Bevestigen", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _closetService.DeleteItem(capturedItem.ItemId);
                    LoadItems();
                }
            };

            btnPanel1.Children.Add(editBtn);
            btnPanel1.Children.Add(deleteBtn);
            stack.Children.Add(btnPanel1);

            var addToColBtn = new Button
            {
                Content = "+ Aan collectie toevoegen",
                FontSize = 10,
                Height = 28,
                Margin = new Thickness(8, 0, 8, 8),
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#F5EFE6")),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")),
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#E0D5C5")),
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            addToColBtn.Click += (s, e) =>
            {
                var collections = _closetService.GetCollections(_currentUser.UserId);
                if (collections.Count == 0)
                {
                    MessageBox.Show("Maak eerst een collectie aan.", "Fout");
                    return;
                }
                var colNames = string.Join("\n",
                    collections.Select((c, i) => $"{i + 1}. {c.Name}"));
                var input = Microsoft.VisualBasic.Interaction.InputBox(
                    $"In welke collectie?\n\n{colNames}\n\nTyp het nummer:",
                    "Collectie kiezen", "1");
                if (int.TryParse(input, out int index) &&
                    index >= 1 && index <= collections.Count)
                {
                    _closetService.AddItemToCollection(
                        capturedItem.ItemId,
                        collections[index - 1].CollectionId);
                    MessageBox.Show(
                        $"Item toegevoegd aan '{collections[index - 1].Name}'!",
                        "Gelukt");
                    LoadItems();
                }
            };

            stack.Children.Add(addToColBtn);
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

                if (!string.IsNullOrEmpty(nameDialog))
                {
                    var myItems = _closetService.GetOrCreateMyItemsCollection(
                        _currentUser.UserId);

                    _closetService.AddItem(
                        _currentUser.UserId,
                        myItems.CollectionId,
                        nameDialog, category, "", "", dest, 0);
                    LoadItems();
                    MessageBox.Show("Item toegevoegd aan 'Mijn items'!", "Gelukt");
                }
            }
        }
    }
}