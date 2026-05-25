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
    public partial class UserProfileWindow : Window
    {
        private User _currentUser;
        private User _profileUser;
        private PostService _postService = new PostService();
        private FollowService _followService = new FollowService();
        private bool _isFollowing = false;

        public UserProfileWindow(User currentUser, User profileUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _profileUser = profileUser;
            LoadProfile();
        }

        private void LoadProfile()
        {
            UsernameText.Text = $"@{_profileUser.Username}";
            BioText.Text = _profileUser.Bio ?? "";

            if (!string.IsNullOrEmpty(_profileUser.ProfilePicturePath) &&
                File.Exists(_profileUser.ProfilePicturePath))
            {
                ProfilePic.Source = new BitmapImage(
                    new Uri(_profileUser.ProfilePicturePath));
            }

            var posts = _postService.GetUserPosts(_profileUser.UserId);
            var followers = _followService.GetFollowerCount(_profileUser.UserId);
            var following = _followService.GetFollowingCount(_profileUser.UserId);

            PostCountText.Text = $"{posts.Count} posts";
            FollowerCountText.Text = $"{followers} volgers";
            FollowingCountText.Text = $"{following} volgend";

            _isFollowing = _followService.IsFollowing(
                _currentUser.UserId, _profileUser.UserId);
            FollowBtn.Content = _isFollowing ? "Ontvolgen" : "Volgen";
            FollowBtn.Background = _isFollowing ?
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#F5EFE6")) :
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#8B6F47"));
            FollowBtn.Foreground = _isFollowing ?
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32")) :
                System.Windows.Media.Brushes.White;

            if (_currentUser.UserId == _profileUser.UserId)
                FollowBtn.Visibility = Visibility.Collapsed;

            foreach (var post in posts)
            {
                var card = CreatePostCard(post);
                PostsPanel.Children.Add(card);
            }
        }

        private void FollowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isFollowing)
            {
                _followService.Unfollow(_currentUser.UserId, _profileUser.UserId);
                _isFollowing = false;
                FollowBtn.Content = "Volgen";
                FollowBtn.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#8B6F47"));
                FollowBtn.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                _followService.Follow(_currentUser.UserId, _profileUser.UserId,
                    _profileUser.PrivacySetting ?? "public");
                _isFollowing = true;
                FollowBtn.Content = "Ontvolgen";
                FollowBtn.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#F5EFE6"));
                FollowBtn.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#5C4A32"));
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
                var tagsPanel = new WrapPanel();
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

            stack.Children.Add(bodyPanel);
            border.Child = stack;
            return border;
        }
    }
}