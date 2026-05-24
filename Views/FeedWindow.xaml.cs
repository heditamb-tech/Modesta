using Modesta.Models;
using Modesta.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Modesta.Views
{
    public partial class FeedWindow : Window
    {
        private User _currentUser;
        private PostService _postService = new PostService();

        public FeedWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadFeed();
        }

        private void LoadFeed()
        {
            FeedPanel.Children.Clear();
            var posts = _postService.GetFeed(_currentUser.UserId);

            if (posts.Count == 0)
            {
                FeedPanel.Children.Add(new TextBlock
                {
                    Text = "Nog geen posts. Volg andere gebruikers om hun posts te zien!",
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
                FeedPanel.Children.Add(card);
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
                    tagBorder.MouseLeftButtonUp += (s, e) =>
                    {
                        var tagInfo = new TagInfoWindow(capturedTag, _currentUser);
                        tagInfo.Show();
                    };

                    tagsPanel.Children.Add(tagBorder);
                }

                bodyPanel.Children.Add(tagsPanel);
            }

            stack.Children.Add(bodyPanel);
            border.Child = stack;
            return border;
        }
    }
}