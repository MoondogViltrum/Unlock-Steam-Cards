using System.Windows;
using System.Windows.Controls;
using SteamCardFarmer.Services;

namespace SteamCardFarmer.Views
{
    public partial class SettingsWindow : Window
    {
        private bool _loading = true;

        public SettingsWindow()
        {
            InitializeComponent();

            // Langues
            foreach (var lang in LocalizationService.AvailableLanguages)
                LangList.Items.Add(new TextBlock { Text = lang, Foreground = System.Windows.Media.Brushes.White, FontSize = 13 });

            for (int i = 0; i < LangList.Items.Count; i++)
                if (LangList.Items[i] is TextBlock tb && tb.Text == LocalizationService.Instance.CurrentLanguage)
                { LangList.SelectedIndex = i; break; }

            // Sons
            foreach (var (name, key) in SoundService.AvailableSounds)
                SoundList.Items.Add(new TextBlock { Text = name, Tag = key, Foreground = System.Windows.Media.Brushes.White, FontSize = 13 });

            SoundEnabledCheck.IsChecked = SoundService.Enabled;

            for (int i = 0; i < SoundList.Items.Count; i++)
                if (SoundList.Items[i] is TextBlock tb && (string)tb.Tag == SoundService.CurrentSound)
                { SoundList.SelectedIndex = i; break; }

            if (SoundList.SelectedIndex < 0) SoundList.SelectedIndex = 0;

            VolumeSlider.Value = SoundService.Volume;
            VolumeLabel.Text = $"{SoundService.Volume}%";
            _loading = false;
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

        private void SoundList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            if (SoundList.SelectedItem is TextBlock tb)
            {
                SoundService.CurrentSound = (string)tb.Tag;
                Properties.Settings.Default.SoundKey = (string)tb.Tag;
                Properties.Settings.Default.Save();
                SoundService.Play(); // Prévisualise le son
            }
        }

        private void Sound_Changed(object sender, RoutedEventArgs e)
        {
            SoundService.Enabled = SoundEnabledCheck.IsChecked == true;
            Properties.Settings.Default.SoundEnabled = SoundService.Enabled;
            Properties.Settings.Default.Save();
        }

        private void Volume_Changed(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (_loading) return;
            var vol = (int)VolumeSlider.Value;
            VolumeLabel.Text = $"{vol}%";
            SoundService.Volume = vol;
            Properties.Settings.Default.SoundVolume = vol;
            Properties.Settings.Default.Save();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();
        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
