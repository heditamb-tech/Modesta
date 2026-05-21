using Modesta.Models;
using System.Windows;

namespace Modesta.Views
{
    public partial class MainAppWindow : Window
    {
        private User _currentUser;

        public MainAppWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            WelcomeText.Text = $"Welkom, @{user.Username}!";
        }
    }
}