using Modesta.Services;
using System.Windows;

namespace Modesta.Views
{
    public partial class RegisterWindow : Window
    {
        private UserService _userService = new UserService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameBox.Text) ||
                string.IsNullOrEmpty(EmailBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Vul alle velden in.", "Fout");
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen.", "Fout");
                return;
            }

            bool success = _userService.Register(
                UsernameBox.Text, EmailBox.Text, PasswordBox.Password);

            if (success)
            {
                MessageBox.Show("Account aangemaakt! Je kan nu inloggen.", "Gelukt");
                var login = new LoginWindow();
                login.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Dit e-mailadres is al in gebruik.", "Fout");
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Hide();
        }
    }
}
