using Modesta.Services;
using System;


using Modesta.Services;
using System.Windows;

namespace Modesta.Views
{
    public partial class LoginWindow : Window
    {
        private UserService _userService = new UserService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EmailBox.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Vul alle velden in.", "Fout");
                return;
            }
            var user = _userService.Login(EmailBox.Text, PasswordBox.Password);
            if (user != null)
            {
                var main = new MainAppWindow(user);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ongeldig e-mailadres of wachtwoord.", "Fout");
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            var reg = new RegisterWindow();
            reg.Show();
            this.Close();
        }
    }
}