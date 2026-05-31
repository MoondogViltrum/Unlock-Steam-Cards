using System.Windows;
using System.Windows.Controls;
using SteamCardFarmer.Services;

namespace SteamCardFarmer.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            foreach (var lang in LocalizationService.AvailableLanguages)
                LangList.Items.Add(new TextBlock
                {
                    Text = lang,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 13
                });

            // Sélectionne la langue actuelle
            for (int i = 0; i < LangList.Items.Count; i++)
            {
                if (LangList.Items[i] is TextBlock tb && tb.Text == LocalizationService.Instance.CurrentLanguage)
                {
                    LangList.SelectedIndex = i;
                    break;
                }
            }
        }

        private void LangList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LangList.SelectedItem is TextBlock tb)
            {
                LocalizationService.Instance.CurrentLanguage = tb.Text;
                Properties.Settings.Default.Language = tb.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
