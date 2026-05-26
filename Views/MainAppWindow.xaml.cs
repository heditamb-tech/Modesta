using Modesta.Models;
using System.Windows;
using System.Windows.Controls;

namespace Modesta.Views
{
    public partial class MainAppWindow : Window
    {
        public User CurrentUser { get; private set; }

        public MainAppWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;
        }

        private void Feed_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var feed = new FeedWindow(CurrentUser);
            feed.Show();
        }
        public void OpenFeed()
        {
            var feed = new FeedWindow(CurrentUser);
            feed.Show();
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var search = new SearchWindow(CurrentUser);
            search.Show();
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var post = new PostWindow(CurrentUser);
            post.Show();
        }

        private void Closet_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var closet = new ClosetWindow(CurrentUser);
            closet.Show();
        }
        private void OutfitBuilder_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var builder = new OutfitBuilderWindow(CurrentUser);
            builder.Show();
        }

        private void AI_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var ai = new AIWindow(CurrentUser);
            ai.Show();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSidebarButton(sender as Button);
            var profile = new ProfileWindow(CurrentUser);
            profile.Show();
        }
        private void Admin_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentUser.IsAdmin)
            {
                SetActiveSidebarButton(sender as Button);
                var admin = new AdminWindow();
                admin.Show();
            }
            else
            {
                MessageBox.Show("Geen toegang.");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void SetActiveSidebarButton(Button active)
        {
            foreach (var child in SidebarPanel.Children)
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
    }
}