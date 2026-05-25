using Modesta.Models;
using Modesta.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Modesta.Views
{
    public partial class SearchWindow : Window
    {
        private User _currentUser;
        private PostService _postService = new PostService();

        public SearchWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text)) return;

            ResultsPanel.Children.Clear();
            var posts = _postService.Search(SearchBox.Text);

            if (posts.Count == 0)
            {
                ResultsPanel.Children.Add(new TextBlock
                {
                    Text = "Geen resultaten gevonden.",
                    FontSize = 13,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7A6B5A")),
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            foreach (var post in posts)
            {
                var card = CreatePostCard(post);
                ResultsPanel.Children.Add(card);
            }
        }

        private Border CreatePostCard(Post post)
        {
            var border = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#E0D5C5")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 0, 16)
            };

            var stack = new StackPanel();

            if (!string.IsNullOrEmpty(post.ImageUrl) && File.Exists(post.ImageUrl))
            {
                var img = new System.Windows.Controls.Image
                {
                    Height = 300,
                    Stretch = System.Windows.Media.Stretch.Uniform
                };
                img.Source = new BitmapImage(new Uri(post.ImageUrl));
                stack.Children.Add(img);
            }

            var bodyPanel = new StackPanel { Margin = new Thickness(16, 12, 16, 12) };

            var usernameBlock = new TextBlock
            {
                Text = $"@{post.User?.Username ?? "onbekend"}",
                FontSize = 12,
                FontWeight = FontWeights.Medium,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#8B6F47")),
                Margin = new Thickness(0, 0, 0, 4),
                Cursor = System.Windows.Input.Cursors.Hand,
                TextDecorations = TextDecorations.Underline
            };

            var capturedUser = post.User;
            usernameBlock.MouseLeftButtonUp += (s, ev) =>
            {
                if (capturedUser != null)
                {
                    var userProfile = new UserProfileWindow(_currentUser, capturedUser);
                    userProfile.Show();
                }
            };

            bodyPanel.Children.Add(usernameBlock);

            bodyPanel.Children.Add(new TextBlock
            {
                Text = post.Caption,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#3A2E22")),
                Margin = new Thickness(0, 0, 0, 8)
            });

            if (post.ItemTags != null)
            {
                var tagsPanel = new WrapPanel { Margin = new Thickness(0, 4, 0, 0) };
                foreach (var tag in post.ItemTags)
                {
                    var tagBorder = new Border
                    {
                        Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                            .ConvertFromString("#F5EFE6")),
                        BorderBrush = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                            .ConvertFromString("#E0D5C5")),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(20),
                        Padding = new Thickness(8, 4, 8, 4),
                        Margin = new Thickness(0, 2, 4, 2),
                        Cursor = System.Windows.Input.Cursors.Hand
                    };

                    tagBorder.Child = new TextBlock
                    {
                        Text = $"🏷️ {tag.ItemType} — {tag.Brand}",
                        FontSize = 11,
                        Foreground = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                            .ConvertFromString("#8B6F47"))
                    };

                    var capturedTag = tag;
                    tagBorder.MouseLeftButtonUp += (s, ev) =>
                    {
                        var tagInfo = new TagInfoWindow(capturedTag, _currentUser);
                        tagInfo.Show();
                    };

                    tagsPanel.Children.Add(tagBorder);
                }
                bodyPanel.Children.Add(tagsPanel);
            }

            var reportBtn = new Button
            {
                Content = "Rapporteer",
                FontSize = 10,
                Padding = new Thickness(8, 4, 8, 4),
                Background = System.Windows.Media.Brushes.Transparent,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#C0392B")),
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#C0392B")),
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 8, 0, 0)
            };

            var capturedPost = post;
            reportBtn.Click += (s, e) =>
            {
                var reason = Microsoft.VisualBasic.Interaction.InputBox(
                    "Waarom rapporteer je deze post?",
                    "Rapporteer post",
                    "Te bloot");
                if (!string.IsNullOrEmpty(reason))
                {
                    _postService.ReportPost(capturedPost.PostId, _currentUser.UserId, reason);
                    MessageBox.Show("Post gerapporteerd!", "Bedankt");
                }
            };

            bodyPanel.Children.Add(reportBtn);
            stack.Children.Add(bodyPanel);
            border.Child = stack;
            return border;
        }
    }
}