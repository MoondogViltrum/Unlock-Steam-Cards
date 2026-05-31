using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using SteamCardFarmer.Models;
using SteamCardFarmer.Services;



namespace SteamCardFarmer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ── Auth ──────────────────────────────────────────────────────────────
        private string _steamLoginSecure = Properties.Settings.Default.SteamLoginSecure;
        private string _sessionId = Properties.Settings.Default.SessionId;
        private string _profileUrl = Properties.Settings.Default.ProfileUrl;
        private string _userName = "";
        private bool _isAuthenticated;

        public string SteamLoginSecure { get => _steamLoginSecure; set { _steamLoginSecure = value; OnPropertyChanged(); } }
        public string SessionId { get => _sessionId; set { _sessionId = value; OnPropertyChanged(); } }
        public string ProfileUrl { get => _profileUrl; set { _profileUrl = value; OnPropertyChanged(); } }
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(); } }
        public bool IsAuthenticated { get => _isAuthenticated; set { _isAuthenticated = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotAuthenticated)); } }
        public bool IsNotAuthenticated => !_isAuthenticated;

        // ── Idle state ────────────────────────────────────────────────────────
        private bool _isIdling;
        private bool _isLoading;
        private bool _isFastMode;
        private Badge? _currentBadge;
        private string _statusText = "Prêt";
        private string _statusDetail = "Connecte-toi pour commencer.";
        private int _checkInterval = 300; // seconds
        private int _timeUntilCheck;

        public bool IsIdling { get => _isIdling; set { _isIdling = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotIdling)); } }
        public bool IsNotIdling => !_isIdling;
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }
        public bool IsFastMode { get => _isFastMode; set { _isFastMode = value; OnPropertyChanged(); } }
        public Badge? CurrentBadge { get => _currentBadge; set { _currentBadge = value; OnPropertyChanged(); } }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public string StatusDetail { get => _statusDetail; set { _statusDetail = value; OnPropertyChanged(); } }
        public int TimeUntilCheck { get => _timeUntilCheck; set { _timeUntilCheck = value; OnPropertyChanged(); OnPropertyChanged(nameof(TimeUntilCheckText)); } }
        public string TimeUntilCheckText => $"{Loc.NextCheck} {TimeUntilCheck}{Loc.Seconds}";

        // ── Badge list ────────────────────────────────────────────────────────
        public ObservableCollection<Badge> Badges { get; } = new();
        private int _totalCards;
        private int _totalGames;
        public int TotalCards { get => _totalCards; set { _totalCards = value; OnPropertyChanged(); } }
        public int TotalGames { get => _totalGames; set { _totalGames = value; OnPropertyChanged(); } }

        // ── Localisation ──────────────────────────────────────────────────────
        public LocalizationService Loc => LocalizationService.Instance;

        // ── Statistics ────────────────────────────────────────────────────────
        public Statistics Stats { get; } = new();

        // ── Internal ──────────────────────────────────────────────────────────
        private readonly DispatcherTimer _checkTimer = new();
        private readonly DispatcherTimer _countdownTimer = new();
        private readonly DispatcherTimer _sessionTimer = new();
        private readonly List<Badge> _fastModeBadges = new();
        private CancellationTokenSource? _cts;
        private readonly string _steamIdlePath;

        public MainViewModel()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _steamIdlePath = Path.Combine(baseDir, "steam-idle.exe");

            // Auto-détecte et copie steam_api64.dll si manquant
            if (!SteamApiHelper.IsReady(baseDir))
            {
                var found = SteamApiHelper.FindAndCopy(baseDir);
                if (found == null)
                {
                    StatusText = "⚠ steam_api64.dll manquant";
                    StatusDetail = "Installe au moins un jeu Steam pour que l'app puisse le trouver automatiquement.";
                }
            }

            _checkTimer.Interval = TimeSpan.FromSeconds(1);
            _checkTimer.Tick += CheckTimer_Tick;

            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += (_, _) =>
            {
                if (TimeUntilCheck > 0) TimeUntilCheck--;
            };

            _sessionTimer.Interval = TimeSpan.FromSeconds(1);
            _sessionTimer.Tick += (_, _) =>
            {
                Stats.TotalIdleTime = Stats.TotalIdleTime.Add(TimeSpan.FromSeconds(1));
            };
        }

        // ── Auth ──────────────────────────────────────────────────────────────
        public async Task LoginAsync()
        {
            IsLoading = true;
            StatusText = "Connexion...";

            var result = await SteamAuthService.AuthenticateAsync(SteamLoginSecure, SessionId);

            if (result.Success)
            {
                ProfileUrl = result.ProfileUrl;
                UserName = result.UserName;
                IsAuthenticated = true;

                Properties.Settings.Default.SteamLoginSecure = SteamLoginSecure;
                Properties.Settings.Default.SessionId = SessionId;
                Properties.Settings.Default.ProfileUrl = ProfileUrl;
                Properties.Settings.Default.Save();

                StatusText = $"Connecté : {UserName}";
                StatusDetail = "Clique sur 'Charger les badges' pour commencer.";
                await LoadBadgesAsync();
            }
            else
            {
                StatusText = "Erreur de connexion";
                StatusDetail = result.Error;
            }

            IsLoading = false;
        }

        public async Task LoadBadgesAsync()
        {
            IsLoading = true;
            StatusText = "Chargement des badges...";
            StatusDetail = "Récupération des jeux avec des cartes restantes...";
            Badges.Clear();

            var badges = await BadgeService.GetBadgesAsync(ProfileUrl);

            foreach (var b in badges)
                Badges.Add(b);

            TotalCards = Badges.Sum(b => b.RemainingCards);
            TotalGames = Badges.Count;

            StatusText = TotalGames > 0
                ? $"{TotalGames} jeu(x) trouvé(s) · {TotalCards} carte(s) restante(s)"
                : "Aucun jeu avec des cartes à farmer";
            StatusDetail = TotalGames > 0 ? "Clique sur 'Démarrer' pour lancer le farming." : "Tous tes jeux sont farmés !";

            IsLoading = false;
        }

        // ── Idle control ──────────────────────────────────────────────────────
        public void StartIdling()
        {
            if (!Badges.Any()) return;
            _cts = new CancellationTokenSource();
            IsIdling = true;
            Stats.ResetSession();
            Stats.TotalIdleTime = TimeSpan.Zero;
            _sessionTimer.Start();

            if (IsFastMode)
                StartFastMode();
            else
                StartSingleMode();
        }

        public void StopIdling()
        {
            _cts?.Cancel();
            _checkTimer.Stop();
            _countdownTimer.Stop();
            _sessionTimer.Stop();

            foreach (var b in Badges) b.StopIdle();
            _fastModeBadges.Clear();

            CurrentBadge = null;
            IsIdling = false;
            StatusText = "Arrêté";
            StatusDetail = "L'idling a été arrêté manuellement.";
        }

        private void StartSingleMode()
        {
            var badge = Badges.FirstOrDefault();
            if (badge == null) { FinishIdling(); return; }

            CurrentBadge = badge;
            try { badge.StartIdle(_steamIdlePath); }
            catch (Exception ex)
            {
                IsIdling = false;
                StatusText = "Erreur";
                StatusDetail = ex.Message;
                return;
            }
            SetCheckInterval(badge.RemainingCards);

            StatusText = $"Idling : {badge.Name}";
            StatusDetail = $"{badge.RemainingCardsText}";

            TimeUntilCheck = _checkInterval;
            _checkTimer.Start();
            _countdownTimer.Start();
        }

        private void StartFastMode()
        {
            // Fast mode: idle all games simultaneously (up to 30)
            var toIdle = Badges.Take(30).ToList();
            _fastModeBadges.Clear();
            _fastModeBadges.AddRange(toIdle);

            foreach (var b in toIdle) b.StartIdle(_steamIdlePath);

            StatusText = $"Fast Mode : {toIdle.Count} jeu(x) en parallèle";
            StatusDetail = "Les jeux avec moins de 2h passeront en fast mode d'abord.";

            TimeUntilCheck = _checkInterval;
            _checkTimer.Start();
            _countdownTimer.Start();
        }

        private void SetCheckInterval(int remainingCards)
        {
            _checkInterval = remainingCards == 1 ? 60 : 300;
        }

        private int _secondsElapsed;
        private async void CheckTimer_Tick(object? sender, EventArgs e)
        {
            _secondsElapsed++;
            if (_secondsElapsed < _checkInterval) return;
            _secondsElapsed = 0;

            _checkTimer.Stop();
            _countdownTimer.Stop();
            StatusDetail = "Vérification des cartes...";

            if (IsFastMode)
                await CheckFastMode();
            else
                await CheckSingleMode();
        }

        private async Task CheckSingleMode()
        {
            if (CurrentBadge == null) return;

            var remaining = await BadgeService.GetRemainingCardsForGame(ProfileUrl, CurrentBadge.AppId);

            if (remaining <= 0)
            {
                // Game done — move to next
                CurrentBadge.StopIdle();
                Stats.AddGame();
                Badges.Remove(CurrentBadge);
                TotalGames = Badges.Count;
                TotalCards = Badges.Sum(b => b.RemainingCards);

                if (!Badges.Any()) { FinishIdling(); return; }
                StartSingleMode();
            }
            else
            {
                var dropped = CurrentBadge.RemainingCards - remaining;
                if (dropped > 0)
                {
                    for (int i = 0; i < dropped; i++) Stats.AddCard();
                    TotalCards -= dropped;
                    ShowNotification($"🃏 Carte droppée !", $"{dropped} carte(s) obtenue(s) dans {CurrentBadge.Name}");
                }

                CurrentBadge.RemainingCards = remaining;
                StatusDetail = CurrentBadge.RemainingCardsText;
                SetCheckInterval(remaining);
                TimeUntilCheck = _checkInterval;
                _checkTimer.Start();
                _countdownTimer.Start();
            }
        }

        private async Task CheckFastMode()
        {
            var finished = new List<Badge>();

            foreach (var badge in _fastModeBadges.ToList())
            {
                var remaining = await BadgeService.GetRemainingCardsForGame(ProfileUrl, badge.AppId);
                if (remaining <= 0)
                {
                    badge.StopIdle();
                    finished.Add(badge);
                    Stats.AddGame();
                }
                else
                {
                    var dropped = badge.RemainingCards - remaining;
                    if (dropped > 0)
                    {
                        for (int i = 0; i < dropped; i++) Stats.AddCard();
                        ShowNotification("🃏 Carte droppée !", $"{dropped} carte(s) dans {badge.Name}");
                    }
                    badge.RemainingCards = remaining;
                }
            }

            foreach (var b in finished)
            {
                _fastModeBadges.Remove(b);
                Badges.Remove(b);
            }

            TotalGames = Badges.Count;
            TotalCards = Badges.Sum(b => b.RemainingCards);

            if (!Badges.Any()) { FinishIdling(); return; }

            StatusText = $"Fast Mode : {_fastModeBadges.Count} jeu(x) actifs";
            StatusDetail = $"{TotalCards} carte(s) restante(s)";
            TimeUntilCheck = _checkInterval;
            _checkTimer.Start();
            _countdownTimer.Start();
        }

        private void FinishIdling()
        {
            IsIdling = false;
            StatusText = "Terminé ! 🎉";
            StatusDetail = $"Tous les jeux ont été farmés. {Stats.TotalCardsDropped} carte(s) obtenue(s).";
            ShowNotification("Steam Card Farmer", "Tous tes jeux ont été farmés !");
        }

        private void ShowNotification(string title, string message)
        {
            // Notify via system tray (handled in MainWindow)
            NotificationRequested?.Invoke(title, message);
        }

        public void SetPriority(Badge badge)
        {
            if (!Badges.Contains(badge)) return;
            Badges.Remove(badge);
            Badges.Insert(0, badge);
            StatusDetail = $"'{badge.Name}' mis en priorité.";
        }

        public event Action<string, string>? NotificationRequested;

        // ── INotifyPropertyChanged ────────────────────────────────────────────
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
