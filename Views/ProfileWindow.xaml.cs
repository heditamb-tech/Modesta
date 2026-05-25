using Microsoft.Win32;
using Modesta.Models;
using Modesta.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Modesta.Views
{
    public partial class ProfileWindow : Window
    {
        private User _currentUser;
        private UserService _userService = new UserService();
        private string _newPhotoPath;

        public ProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadProfile();
        }

        private void LoadProfile()
        {
            UsernameBox.Text = _currentUser.Username;
            BioBox.Text = _currentUser.Bio;
            if (!string.IsNullOrEmpty(_currentUser.ProfilePicturePath)
                && File.Exists(_currentUser.ProfilePicturePath))
            {
                ProfilePicture.Source = new BitmapImage(
                    new System.Uri(_currentUser.ProfilePicturePath));
            }

            foreach (ComboBoxItem item in ThemeCombo.Items)
            {
                if (item.Tag.ToString() == (_currentUser.UITheme ?? "beige"))
                {
                    ThemeCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png";
            if (dialog.ShowDialog() == true)
            {
                string folder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Photos");
                Directory.CreateDirectory(folder);
                string filename = Guid.NewGuid()
                    + Path.GetExtension(dialog.FileName);
                string dest = Path.Combine(folder, filename);
                File.Copy(dialog.FileName, dest);
                _newPhotoPath = dest;
                ProfilePicture.Source = new BitmapImage(new System.Uri(dest));
            }
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            _userService.UpdateProfile(
                _currentUser.UserId,
                BioBox.Text,
                _newPhotoPath,
                UsernameBox.Text);

            if (!string.IsNullOrEmpty(NewPasswordBox.Password))
                _userService.ChangePassword(
                    _currentUser.UserId, NewPasswordBox.Password);

            var selectedTheme = (ThemeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "beige";
            _userService.UpdateProfile(_currentUser.UserId, BioBox.Text, _newPhotoPath, UsernameBox.Text);
            // Update theme
            using (var db = new Modesta.Data.ModestDbContext())
            {
                var user = db.Users.Find(_currentUser.UserId);
                if (user != null)
                {
                    user.UITheme = selectedTheme;
                    db.SaveChanges();
                }
            }
            ThemeService.ApplyTheme(selectedTheme);

            MessageBox.Show("Profiel opgeslagen!", "Gelukt");
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Ben je zeker? Dit kan niet ongedaan gemaakt worden.",
                "Account verwijderen",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                _userService.DeleteAccount(_currentUser.UserId);
                var login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}