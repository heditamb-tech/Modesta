using Modesta.Services;
using System.Windows;
using System;

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
            try
            {
                if (string.IsNullOrEmpty(EmailBox.Text) ||
                    string.IsNullOrEmpty(PasswordBox.Password))
                {
                    MessageBox.Show("Vul alle velden in.", "Fout");
                    return;
                }
                var user = _userService.Login(EmailBox.Text, PasswordBox.Password);
                if (user != null)
                {
                    MainAppWindow main = new MainAppWindow(user);
                    Application.Current.MainWindow = main;
                    ThemeService.ApplyTheme(user.UITheme ?? "beige");
                    main.Show();
                    this.Close();
                }
                else
                {
                    this.Activate();
                    MessageBox.Show("Ongeldig e-mailadres of wachtwoord.", "Fout",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FOUT: " + ex.Message + "\n\n" + ex.InnerException?.Message,
                                "Debug fout");
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
