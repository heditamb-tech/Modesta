using Modesta.Models;
using Modesta.Services;
using System.Windows;
using System.Windows.Controls;
using System;

namespace Modesta.Views
{
    public partial class AIWindow : Window
    {
        private User _currentUser;
        private AIService _aiService = new AIService();

        public AIWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(QuestionBox.Text)) return;

            var question = QuestionBox.Text;
            QuestionBox.Text = "";

            AddMessage(question, isUser: true);

            try
            {
                var response = await _aiService.GetOutfitSuggestion(
                    _currentUser.UserId, question);
                AddMessage(response, isUser: false);
                ChatScroll.ScrollToBottom();
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI fout: " + ex.Message + "\n" + ex.InnerException?.Message);
            }
        }

        private void AddMessage(string text, bool isUser)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(0, 0, 0, 8),
                MaxWidth = 400,
                HorizontalAlignment = isUser ?
                    HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString(isUser ? "#F5EFE6" : "#FDFAF6")),
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#E0D5C5")),
                BorderThickness = new Thickness(1)
            };

            border.Child = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#3A2E22"))
            };

            ChatPanel.Children.Add(border);
        }
    }
}