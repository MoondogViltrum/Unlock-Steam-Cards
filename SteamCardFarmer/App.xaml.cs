using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SteamCardFarmer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ensure single instance
            var existing = System.Diagnostics.Process.GetProcessesByName("SteamCardFarmer");
            if (existing.Length > 1)
            {
                MessageBox.Show("Steam Card Farmer est déjà lancé.", "Steam Card Farmer", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }
        }
    }
}
