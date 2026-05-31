using System.Windows;
using System.Windows.Controls;
using SteamCardFarmer.Models;
using SteamCardFarmer.Services;
using System.Collections.Generic;

namespace SteamCardFarmer.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(IEnumerable<HistoryEntry> history)
        {
            InitializeComponent();
            HistoryList.ItemsSource = history;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            HistoryService.Clear();
            HistoryList.ItemsSource = null;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
