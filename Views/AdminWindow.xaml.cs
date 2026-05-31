using Modesta.Models;
using Modesta.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Modesta.Views
{
    public partial class AdminWindow : Window
    {
        private AdminService _adminService = new AdminService();

        public AdminWindow()
        {
            InitializeComponent();
            LoadStats();
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadUsers();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadReports();
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadStats();
        }

        private void SetActiveButton(Button active)
        {
            var sidebar = active.Parent as StackPanel;
            if (sidebar == null) return;
            foreach (var child in sidebar.Children)
            {
                if (child is Button btn)
                {
                    btn.Background = System.Windows.Media.Brushes.Transparent;
                    btn.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7A6B5A"));
                }
            }
            active.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                .ConvertFromString("#F5EFE6"));
            active.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                .ConvertFromString("#5C4A32"));
        }

        private void LoadStats()
        {
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Statistieken",
                FontSize = 18,
                FontFamily = new System.Windows.Media.FontFamily("Georgia"),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")),
                Margin = new Thickness(0, 0, 0, 20)
            });

            var userCount = _adminService.GetUserCount();
            var postCount = _adminService.GetPostCount();
            var reportCount = _adminService.GetPendingReportCount();
            var max = Math.Max(Math.Max(userCount, postCount), Math.Max(reportCount, 1));

            // Stat cards
            var stats = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 24) };
            stats.Children.Add(CreateStatCard("Gebruikers", userCount.ToString()));
            stats.Children.Add(CreateStatCard("Posts", postCount.ToString()));
            stats.Children.Add(CreateStatCard("Rapporten", reportCount.ToString()));
            ContentPanel.Children.Add(stats);

            // Grafiek titel
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Overzicht",
                FontSize = 14,
                FontWeight = FontWeights.Medium,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#3A2E22")),
                Margin = new Thickness(0, 0, 0, 12)
            });

            // Balkjes
            var chart = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };
            chart.Children.Add(CreateBar("Gebruikers", userCount, max, "#8B6F47"));
            chart.Children.Add(CreateBar("Posts", postCount, max, "#C4A882"));
            chart.Children.Add(CreateBar("Rapporten", reportCount, max, "#FAECE7"));
            ContentPanel.Children.Add(chart);
        }

        private Border CreateBar(string label, int value, int max, string color)
        {
            var container = new Grid { Margin = new Thickness(0, 0, 0, 8) };
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

            var labelBlock = new TextBlock
            {
                Text = label,
                FontSize = 12,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#7A6B5A")),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(labelBlock, 0);

            var barWidth = max == 0 ? 0 : (double)value / max;
            var barContainer = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#F5EFE6")),
                Height = 28,
                CornerRadius = new CornerRadius(4)
            };

            var bar = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString(color)),
                Height = 28,
                CornerRadius = new CornerRadius(4),
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = barWidth * 400
            };
            barContainer.Child = bar;
            Grid.SetColumn(barContainer, 1);

            var valueBlock = new TextBlock
            {
                Text = value.ToString(),
                FontSize = 12,
                FontWeight = FontWeights.Medium,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#3A2E22")),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(valueBlock, 2);

            container.Children.Add(labelBlock);
            container.Children.Add(barContainer);
            container.Children.Add(valueBlock);

            return new Border { Child = container, Margin = new Thickness(0, 0, 0, 4) };
        }

        private Border CreateStatCard(string label, string value)
        {
            var border = new Border
            {
                Width = 150,
                Height = 100,
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#F5EFE6")),
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#E0D5C5")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 0, 12, 0)
            };
            var stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            stack.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 28,
                FontWeight = FontWeights.Medium,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#3A2E22"))
            });
            stack.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#7A6B5A"))
            });
            border.Child = stack;
            return border;
        }

        private void LoadUsers()
        {
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Gebruikers",
                FontSize = 18,
                FontFamily = new System.Windows.Media.FontFamily("Georgia"),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")),
                Margin = new Thickness(0, 0, 0, 16)
            });

            var users = _adminService.GetAllUsers();
            foreach (var user in users)
            {
                var row = new Border
                {
                    Background = System.Windows.Media.Brushes.White,
                    BorderBrush = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#E0D5C5")),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 10, 16, 10),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var panel = new Grid();
                panel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = new GridLength(1, GridUnitType.Star) });
                panel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = GridLength.Auto });

                var info = new TextBlock
                {
                    Text = $"@{user.Username} — {user.Email}",
                    FontSize = 13,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#3A2E22"))
                };
                Grid.SetColumn(info, 0);

                var deleteBtn = new Button
                {
                    Content = "Verwijderen",
                    Width = 100,
                    Padding = new Thickness(12, 6, 12, 6),
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#FAECE7")),
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#712B13")),
                    BorderThickness = new Thickness(0),
                    FontSize = 11,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = user.UserId
                };
                deleteBtn.Click += (s, e) =>
                {
                    var result = MessageBox.Show(
                        $"Gebruiker @{user.Username} verwijderen?",
                        "Bevestigen", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        _adminService.DeleteUser(user.UserId);
                        LoadUsers();
                    }
                };
                Grid.SetColumn(deleteBtn, 1);

                panel.Children.Add(info);
                panel.Children.Add(deleteBtn);
                row.Child = panel;
                ContentPanel.Children.Add(row);
            }
        }

        private void LoadReports()
        {
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Gerapporteerde posts",
                FontSize = 18,
                FontFamily = new System.Windows.Media.FontFamily("Georgia"),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")),
                Margin = new Thickness(0, 0, 0, 16)
            });

            var reports = _adminService.GetPendingReports();

            if (reports.Count == 0)
            {
                ContentPanel.Children.Add(new TextBlock
                {
                    Text = "Geen openstaande rapporten.",
                    FontSize = 13,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7A6B5A"))
                });
                return;
            }

            foreach (var report in reports)
            {
                var row = new Border
                {
                    Background = System.Windows.Media.Brushes.White,
                    BorderBrush = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#E0D5C5")),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 10, 16, 10),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var panel = new Grid();
                panel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = new GridLength(1, GridUnitType.Star) });
                panel.ColumnDefinitions.Add(new ColumnDefinition
                { Width = GridLength.Auto });

                var info = new StackPanel();

                info.Children.Add(new TextBlock
                {
                    Text = $"Gepost door: @{report.Post?.User?.Username ?? "onbekend"}",
                    FontSize = 13,
                    FontWeight = FontWeights.Medium,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#3A2E22")),
                    Margin = new Thickness(0, 0, 0, 4)
                });

                info.Children.Add(new TextBlock
                {
                    Text = $"Reden: {report.Reason}",
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7A6B5A")),
                    Margin = new Thickness(0, 0, 0, 4)
                });

                if (!string.IsNullOrEmpty(report.Post?.ImageUrl) &&
                    System.IO.File.Exists(report.Post.ImageUrl))
                {
                    var img = new System.Windows.Controls.Image
                    {
                        Height = 120,
                        Width = 120,
                        Stretch = System.Windows.Media.Stretch.Uniform,
                        Margin = new Thickness(0, 4, 0, 4)
                    };
                    img.Source = new System.Windows.Media.Imaging.BitmapImage(
                        new Uri(report.Post.ImageUrl));
                    info.Children.Add(img);
                }

                info.Children.Add(new TextBlock
                {
                    Text = $"Gepost op: {report.Post?.CreatedAt.ToString("dd/MM/yyyy")}",
                    FontSize = 11,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7A6B5A"))
                });

                Grid.SetColumn(info, 0);

                var btnPanel = new StackPanel { Orientation = Orientation.Horizontal };

                var removeBtn = new Button
                {
                    Content = "Verwijderen",
                    Width = 100,
                    Padding = new Thickness(10, 6, 10, 6),
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#FAECE7")),
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#712B13")),
                    BorderThickness = new Thickness(0),
                    FontSize = 11,
                    Margin = new Thickness(0, 0, 8, 0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                removeBtn.Click += (s, e) =>
                {
                    _adminService.RemoveReportedPost(report.ReportId);
                    LoadReports();
                };

                var ignoreBtn = new Button
                {
                    Content = "Negeren",
                    Width = 100,
                    Padding = new Thickness(10, 6, 10, 6),
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#E8F0E8")),
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#3A5A3A")),
                    BorderThickness = new Thickness(0),
                    FontSize = 11,
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                ignoreBtn.Click += (s, e) =>
                {
                    _adminService.IgnoreReport(report.ReportId);
                    LoadReports();
                };

                btnPanel.Children.Add(removeBtn);
                btnPanel.Children.Add(ignoreBtn);
                Grid.SetColumn(btnPanel, 1);

                panel.Children.Add(info);
                panel.Children.Add(btnPanel);
                row.Child = panel;
                ContentPanel.Children.Add(row);
            }
        }
    }
}