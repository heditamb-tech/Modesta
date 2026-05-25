using Modesta.Models;
using System.Windows;

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
            var feed = new FeedWindow(CurrentUser);
            feed.Show();
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var search = new SearchWindow(CurrentUser);
            search.Show();
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            var post = new PostWindow(CurrentUser);
            post.Show();
        }

        private void Closet_Click(object sender, RoutedEventArgs e)
        {
            var closet = new ClosetWindow(CurrentUser);
            closet.Show();
        }

        private void AI_Click(object sender, RoutedEventArgs e)
        {
            var ai = new AIWindow(CurrentUser);
            ai.Show();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new ProfileWindow(CurrentUser);
            profile.Show();
        }
        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            
            if (CurrentUser.IsAdmin)
            {
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
    }
}