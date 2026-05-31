using System.Windows;
using WF = System.Windows.Forms;
using SteamCardFarmer.ViewModels;
using SteamCardFarmer.Services;

namespace SteamCardFarmer.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;
        private WF.NotifyIcon? _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainViewModel)DataContext;
            _vm.NotificationRequested += ShowNotification;
            InitTrayIcon();

            // Restaure la langue sauvegardée
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Language))
                LocalizationService.Instance.CurrentLanguage = Properties.Settings.Default.Language;

            // Auto-login if cookies saved
            if (!string.IsNullOrEmpty(_vm.SteamLoginSecure) && !string.IsNullOrEmpty(_vm.SessionId))
                Loaded += async (_, _) => await _vm.LoginAsync();
        }

        // ── Window controls ───────────────────────────────────────────────────
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            else
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _vm.StopIdling();
            _trayIcon?.Dispose();
            Close();
        }

        // ── Actions ───────────────────────────────────────────────────────────
        private async void Login_Click(object sender, RoutedEventArgs e)
            => await _vm.LoginAsync();

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _vm.StopIdling();
            Properties.Settings.Default.SteamLoginSecure = "";
            Properties.Settings.Default.SessionId = "";
            Properties.Settings.Default.Save();
            _vm.IsAuthenticated = false;
            _vm.Badges.Clear();
            _vm.UserName = "";
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
            => await _vm.LoadBadgesAsync();

        private void Start_Click(object sender, RoutedEventArgs e)
            => _vm.StartIdling();

        private void Stop_Click(object sender, RoutedEventArgs e)
            => _vm.StopIdling();

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow { Owner = this };
            win.ShowDialog();
        }

        private void PriorityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is SteamCardFarmer.Models.Badge badge)
                _vm.SetPriority(badge);
        }

        // ── System tray ───────────────────────────────────────────────────────
        private void InitTrayIcon()
        {
            _trayIcon = new WF.NotifyIcon
            {
                Text = "Unlock Steam Cards",
                Visible = true
            };

            try
            {
                var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.ico");
                if (System.IO.File.Exists(iconPath))
                    _trayIcon.Icon = new System.Drawing.Icon(iconPath);
                else
                    _trayIcon.Icon = System.Drawing.SystemIcons.Application;
            }
            catch { _trayIcon.Icon = System.Drawing.SystemIcons.Application; }

            var menu = new WF.ContextMenuStrip();
            menu.Items.Add("Ouvrir", null, (_, _) => { Show(); WindowState = WindowState.Normal; Activate(); });
            menu.Items.Add("Quitter", null, (_, _) =>
            {
                _vm.StopIdling();
                _trayIcon?.Dispose();
                System.Windows.Application.Current.Shutdown();
            });
            _trayIcon.ContextMenuStrip = menu;
            _trayIcon.DoubleClick += (_, _) => { Show(); WindowState = WindowState.Normal; Activate(); };

            StateChanged += (_, _) =>
            {
                if (WindowState == WindowState.Minimized)
                {
                    Hide();
                    _trayIcon.ShowBalloonTip(2000, "Steam Card Farmer", "Réduit dans la barre des tâches.", WF.ToolTipIcon.Info);
                }
            };
        }

        private void ShowNotification(string title, string message)
        {
            Dispatcher.Invoke(() =>
                _trayIcon?.ShowBalloonTip(3000, title, message, WF.ToolTipIcon.Info));
        }

        protected override void OnClosed(EventArgs e)
        {
            _trayIcon?.Dispose();
            base.OnClosed(e);
        }
    }
}
