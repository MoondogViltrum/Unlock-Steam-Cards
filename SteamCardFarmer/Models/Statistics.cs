using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteamCardFarmer.Models
{
    public class Statistics : INotifyPropertyChanged
    {
        private int _totalCardsDropped;
        private int _totalGamesIdled;
        private TimeSpan _totalIdleTime;
        private DateTime _sessionStart = DateTime.Now;

        public int TotalCardsDropped
        {
            get => _totalCardsDropped;
            set { _totalCardsDropped = value; OnPropertyChanged(); }
        }

        public int TotalGamesIdled
        {
            get => _totalGamesIdled;
            set { _totalGamesIdled = value; OnPropertyChanged(); }
        }

        public TimeSpan TotalIdleTime
        {
            get => _totalIdleTime;
            set { _totalIdleTime = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalIdleTimeText)); }
        }

        public string TotalIdleTimeText => $"{(int)TotalIdleTime.TotalHours:D2}:{TotalIdleTime.Minutes:D2}:{TotalIdleTime.Seconds:D2}";

        public TimeSpan SessionDuration => DateTime.Now - _sessionStart;

        public void AddCard() => TotalCardsDropped++;
        public void AddGame() => TotalGamesIdled++;
        public void ResetSession() => _sessionStart = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
