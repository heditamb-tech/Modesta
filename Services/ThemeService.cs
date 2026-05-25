using System;
using System.Windows;

namespace Modesta.Services
{
    public static class ThemeService
    {
        public static void ApplyTheme(string theme)
        {
            var dict = new ResourceDictionary();

            switch (theme)
            {
                case "sage":
                    dict.Source = new Uri("Themes/SageGreen.xaml",
                        UriKind.Relative);
                    break;
                case "pink":
                    dict.Source = new Uri("Themes/BabyPink.xaml",
                        UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Themes/ClassicBeige.xaml",
                        UriKind.Relative);
                    break;
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}