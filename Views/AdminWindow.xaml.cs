using Modesta.Models;
using Modesta.Services;
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

        private void Users_Click(object sender, RoutedEventArgs e) => LoadUsers();
        private void Reports_Click(object sender, RoutedEventArgs e) => LoadReports();
        private void Stats_Click(object sender, RoutedEventArgs e) => LoadStats();

        private void LoadStats()
        {
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Statistieken",
                FontSize = 18, 
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")),
                Margin = new Thickness(0, 0, 0, 16)
            });

            var stats = new StackPanel { Orientation = Orientation.Horizontal };

            stats.Children.Add(CreateStatCard("Gebruikers",
                _adminService.GetUserCount().ToString()));
            stats.Children.Add(CreateStatCard("Posts",
                _adminService.GetPostCount().ToString()));
            stats.Children.Add(CreateStatCard("Rapporten",
                _adminService.GetPendingReportCount().ToString()));

            ContentPanel.Children.Add(stats);
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
                panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

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
                panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var info = new StackPanel();
                info.Children.Add(new TextBlock
                {
                    Text = $"Reden: {report.Reason}",
                    FontSize = 13,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#3A2E22"))
                });
                Grid.SetColumn(info, 0);

                var btnPanel = new StackPanel
                { Orientation = Orientation.Horizontal };

                var removeBtn = new Button
                {
                    Content = "Verwijderen",
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