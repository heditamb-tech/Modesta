using Modesta.Models;
using Modesta.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Modesta.Views
{
    public partial class OutfitBuilderWindow : Window
    {
        private User _currentUser;
        private ClosetService _closetService = new ClosetService();
        private Dictionary<string, ClothingItem> _selectedItems = new Dictionary<string, ClothingItem>();

        public OutfitBuilderWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadItems();
        }

        private void LoadItems()
        {
            ItemsSelector.Children.Clear();
            var allItems = _closetService.GetItems(_currentUser.UserId);

            // Filter items uit de "Outfit builder" collectie eruit
            var items = allItems
                .Where(i => i.Collection?.Name != "Outfit builder")
                .ToList();

            if (items.Count == 0)
            {
                ItemsSelector.Children.Add(new TextBlock
                {
                    Text = "Voeg eerst items toe aan je kledingkast.",
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7A6B5A")),
                    TextWrapping = TextWrapping.Wrap
                });
                return;
            }

            // rest blijft hetzelfde

            ItemsSelector.Children.Add(new TextBlock
            {
                Text = "Selecteer items",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#8B6F47")),
                Margin = new Thickness(0, 0, 0, 8)
            });

            foreach (var item in items)
            {
                var itemRow = new Border
                {
                    Background = Brushes.Transparent,
                    BorderBrush = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#E0D5C5")),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 4),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var rowPanel = new Grid();
                rowPanel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = new GridLength(40) });
                rowPanel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = new GridLength(1, GridUnitType.Star) });
                rowPanel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = new GridLength(20) });

                if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
                {
                    var img = new Image
                    {
                        Width = 36,
                        Height = 36,
                        Stretch = Stretch.UniformToFill
                    };
                    img.Source = new BitmapImage(new Uri(item.PhotoPath));
                    Grid.SetColumn(img, 0);
                    rowPanel.Children.Add(img);
                }
                else
                {
                    var placeholder = new Border
                    {
                        Width = 36,
                        Height = 36,
                        Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#E8DDD0")),
                        CornerRadius = new CornerRadius(4)
                    };
                    Grid.SetColumn(placeholder, 0);
                    rowPanel.Children.Add(placeholder);
                }

                var infoPanel = new StackPanel
                {
                    Margin = new Thickness(8, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                infoPanel.Children.Add(new TextBlock
                {
                    Text = item.Name,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#3A2E22"))
                });
                infoPanel.Children.Add(new TextBlock
                {
                    Text = item.Category ?? "",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7A6B5A"))
                });
                Grid.SetColumn(infoPanel, 1);
                rowPanel.Children.Add(infoPanel);

                var checkText = new TextBlock
                {
                    Text = "",
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#8B6F47"))
                };
                Grid.SetColumn(checkText, 2);
                rowPanel.Children.Add(checkText);

                itemRow.Child = rowPanel;

                var capturedItem = item;
                var capturedCheck = checkText;
                var capturedRow = itemRow;

                itemRow.MouseLeftButtonUp += (s, e) =>
                {
                    var layerKey = GetLayerKey(capturedItem.Category ?? "");

                    if (_selectedItems.ContainsKey(layerKey) &&
                        _selectedItems[layerKey].ItemId == capturedItem.ItemId)
                    {
                        _selectedItems.Remove(layerKey);
                        capturedCheck.Text = "";
                        capturedRow.Background = Brushes.Transparent;
                        ResetLayer(layerKey);
                    }
                    else
                    {
                        _selectedItems[layerKey] = capturedItem;
                        capturedCheck.Text = "✓";
                        capturedRow.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F5EFE6"));
                        UpdateLayer(layerKey, capturedItem);
                    }
                };

                ItemsSelector.Children.Add(itemRow);
            }
        }

        private string GetLayerKey(string category)
        {
            var cat = category.ToLower();
            if (cat.Contains("hoofd") || cat.Contains("hijab") ||
                cat.Contains("sjaal") || cat.Contains("hoofdbedekking"))
                return "hijab";
            if (cat.Contains("schoen") || cat.Contains("laars") ||
                cat.Contains("sneaker") || cat.Contains("hakken"))
                return "shoes";
            if (cat.Contains("tas") || cat.Contains("bag") ||
                cat.Contains("rugzak") || cat.Contains("clutch"))
                return "bag";
            if (cat.Contains("rok") || cat.Contains("broek") ||
                cat.Contains("jeans") || cat.Contains("legging"))
                return "bottom";
            return "top";
        }

        private void UpdateLayer(string layerKey, ClothingItem item)
        {
            var color = GetColorFromName(item.Color ?? "");

            switch (layerKey)
            {
                case "hijab":
                    HijabLayer.Visibility = Visibility.Visible;
                    if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
                    {
                        HijabLayer.Fill = new ImageBrush(
                            new BitmapImage(new Uri(item.PhotoPath)))
                        { Stretch = Stretch.UniformToFill };
                    }
                    else
                    {
                        HijabLayer.Fill = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(color));
                    }
                    break;
                case "top":
                    if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
                    {
                        TopLayer.Fill = new ImageBrush(
                            new BitmapImage(new Uri(item.PhotoPath)))
                        { Stretch = Stretch.UniformToFill };
                    }
                    else
                    {
                        TopLayer.Fill = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(color));
                    }
                    break;
                case "bottom":
                    if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
                    {
                        BottomLayer.Fill = new ImageBrush(
                            new BitmapImage(new Uri(item.PhotoPath)))
                        { Stretch = Stretch.UniformToFill };
                    }
                    else
                    {
                        BottomLayer.Fill = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(color));
                    }
                    break;
                case "shoes":
                    if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
                    {
                        var brush = new ImageBrush(
                            new BitmapImage(new Uri(item.PhotoPath)))
                        { Stretch = Stretch.UniformToFill };
                        ShoesLayer.Fill = brush;
                        ShoesLayer2.Fill = brush;
                    }
                    else
                    {
                        ShoesLayer.Fill = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(color));
                        ShoesLayer2.Fill = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(color));
                    }
                    break;
                case "bag":
                    BagLayer.Visibility = Visibility.Visible;
                    if (!string.IsNullOrEmpty(item.PhotoPath) && File.Exists(item.PhotoPath))
                    {
                        BagLayer.Fill = new ImageBrush(
                            new BitmapImage(new Uri(item.PhotoPath)))
                        { Stretch = Stretch.UniformToFill };
                    }
                    else
                    {
                        BagLayer.Fill = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(color));
                    }
                    break;
            }
        }

        private void ResetLayer(string layerKey)
        {
            switch (layerKey)
            {
                case "hijab":
                    HijabLayer.Visibility = Visibility.Collapsed;
                    break;
                case "top":
                    TopLayer.Fill = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#E8DDD0"));
                    break;
                case "bottom":
                    BottomLayer.Fill = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#E8DDD0"));
                    break;
                case "shoes":
                    ShoesLayer.Fill = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#D4C4B0"));
                    ShoesLayer2.Fill = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#D4C4B0"));
                    break;
                case "bag":
                    BagLayer.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private string GetColorFromName(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "beige": return "#E8DDD0";
                case "wit": case "white": return "#F0EDE8";
                case "zwart": case "black": return "#2A2A2A";
                case "bruin": case "brown": return "#8B6F47";
                case "grijs": case "grey": case "gray": return "#9A9A9A";
                case "roze": case "pink": return "#FFB6C1";
                case "rood": case "red": return "#C0392B";
                case "blauw": case "blue": return "#2980B9";
                case "groen": case "green": return "#27AE60";
                case "geel": case "yellow": return "#F1C40F";
                case "paars": case "purple": return "#8E44AD";
                case "oranje": case "orange": return "#E67E22";
                case "crème": case "creme": return "#FFFDD0";
                case "camel": return "#C19A6B";
                case "khaki": return "#C3B091";
                case "sage": return "#8FAF8F";
                default: return "#C4A882";
            }
        }

        private void SaveOutfit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedItems.Count == 0)
            {
                MessageBox.Show("Selecteer minstens één item.", "Fout");
                return;
            }

            var name = Microsoft.VisualBasic.Interaction.InputBox(
                "Geef je outfit een naam:", "Outfit opslaan", "Mijn outfit");

            if (!string.IsNullOrEmpty(name))
            {
                // Haal de "Outfit builder" collectie op of maak die aan
                var outfitCol = _closetService.GetOrCreateOutfitBuilderCollection(
                    _currentUser.UserId);

                foreach (var item in _selectedItems.Values)
                {
                    _closetService.AddItem(
                        _currentUser.UserId,
                        outfitCol.CollectionId,
                        $"{name} — {item.Name}",
                        item.Category,
                        item.Color,
                        item.Brand,
                        item.PhotoPath,
                        item.Price);
                }

                MessageBox.Show($"Outfit '{name}' opgeslagen in je kledingkast!", "Gelukt");
            }
        }
    }
}
