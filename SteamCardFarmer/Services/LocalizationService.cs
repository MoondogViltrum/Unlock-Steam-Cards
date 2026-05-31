using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteamCardFarmer.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private static LocalizationService? _instance;
        public static LocalizationService Instance => _instance ??= new LocalizationService();

        private string _currentLanguage = "Français";
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set { _currentLanguage = value; OnPropertyChanged(); OnAllChanged(); }
        }

        public static readonly List<string> AvailableLanguages = new()
        {
            "Français", "English", "Español", "Deutsch", "Italiano",
            "中文", "Português", "日本語"
        };

        // Toutes les chaînes de l'interface
        private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
        {
            ["Français"] = new()
            {
                ["auth"] = "AUTHENTIFICATION",
                ["connected"] = "✓ Connecté",
                ["disconnect"] = "↩  Déconnexion",
                ["connect"] = "🔑  Se connecter",
                ["options"] = "OPTIONS",
                ["fast_mode"] = "Fast Mode",
                ["fast_mode_desc"] = "Idle plusieurs jeux en parallèle.",
                ["stats"] = "STATISTIQUES SESSION",
                ["cards_farmed"] = "Cartes farmées",
                ["games_done"] = "Jeux terminés",
                ["session_time"] = "Temps de session",
                ["refresh"] = "🔄  Actualiser",
                ["start"] = "▶  Démarrer",
                ["stop"] = "⏹  Arrêter",
                ["image"] = "IMAGE",
                ["game"] = "JEU",
                ["cards"] = "CARTES",
                ["hours"] = "HEURES",
                ["prio"] = "PRIO",
                ["idling"] = "EN COURS D'IDLING",
                ["connect_to_see"] = "Connecte-toi pour voir tes badges",
                ["ready"] = "Prêt",
                ["connect_to_start"] = "Connecte-toi pour commencer.",
                ["how_to_cookies"] = "Comment obtenir les cookies ?\n1. Ouvre Steam dans Chrome\n2. F12 → Application → Cookies\n3. steamcommunity.com\n4. Copie steamLoginSecure et sessionid",
                ["settings"] = "PARAMÈTRES",
                ["language"] = "Langue",
                ["next_check"] = "Prochain check dans",
                ["seconds"] = "s",
                ["games"] = "jeu(x)",
                ["cards_count"] = "carte(s)",
                ["stopped"] = "Arrêté",
                ["stopped_manual"] = "L'idling a été arrêté manuellement.",
                ["finished"] = "Terminé ! 🎉",
                ["priority_set"] = "mis en priorité.",
            },
            ["English"] = new()
            {
                ["auth"] = "AUTHENTICATION",
                ["connected"] = "✓ Connected",
                ["disconnect"] = "↩  Disconnect",
                ["connect"] = "🔑  Sign in",
                ["options"] = "OPTIONS",
                ["fast_mode"] = "Fast Mode",
                ["fast_mode_desc"] = "Idle multiple games at once.",
                ["stats"] = "SESSION STATISTICS",
                ["cards_farmed"] = "Cards farmed",
                ["games_done"] = "Games done",
                ["session_time"] = "Session time",
                ["refresh"] = "🔄  Refresh",
                ["start"] = "▶  Start",
                ["stop"] = "⏹  Stop",
                ["image"] = "IMAGE",
                ["game"] = "GAME",
                ["cards"] = "CARDS",
                ["hours"] = "HOURS",
                ["prio"] = "PRIO",
                ["idling"] = "CURRENTLY IDLING",
                ["connect_to_see"] = "Sign in to see your badges",
                ["ready"] = "Ready",
                ["connect_to_start"] = "Sign in to start.",
                ["how_to_cookies"] = "How to get cookies?\n1. Open Steam in Chrome\n2. F12 → Application → Cookies\n3. steamcommunity.com\n4. Copy steamLoginSecure and sessionid",
                ["settings"] = "SETTINGS",
                ["language"] = "Language",
                ["next_check"] = "Next check in",
                ["seconds"] = "s",
                ["games"] = "game(s)",
                ["cards_count"] = "card(s)",
                ["stopped"] = "Stopped",
                ["stopped_manual"] = "Idling was stopped manually.",
                ["finished"] = "Done! 🎉",
                ["priority_set"] = "set as priority.",
            },
            ["Español"] = new()
            {
                ["auth"] = "AUTENTICACIÓN",
                ["connected"] = "✓ Conectado",
                ["disconnect"] = "↩  Desconectar",
                ["connect"] = "🔑  Conectarse",
                ["options"] = "OPCIONES",
                ["fast_mode"] = "Modo Rápido",
                ["fast_mode_desc"] = "Idlear varios juegos a la vez.",
                ["stats"] = "ESTADÍSTICAS DE SESIÓN",
                ["cards_farmed"] = "Cartas obtenidas",
                ["games_done"] = "Juegos terminados",
                ["session_time"] = "Tiempo de sesión",
                ["refresh"] = "🔄  Actualizar",
                ["start"] = "▶  Iniciar",
                ["stop"] = "⏹  Detener",
                ["image"] = "IMAGEN",
                ["game"] = "JUEGO",
                ["cards"] = "CARTAS",
                ["hours"] = "HORAS",
                ["prio"] = "PRIO",
                ["idling"] = "EJECUTANDO",
                ["connect_to_see"] = "Conéctate para ver tus insignias",
                ["ready"] = "Listo",
                ["connect_to_start"] = "Conéctate para empezar.",
                ["how_to_cookies"] = "¿Cómo obtener las cookies?\n1. Abre Steam en Chrome\n2. F12 → Aplicación → Cookies\n3. steamcommunity.com\n4. Copia steamLoginSecure y sessionid",
                ["settings"] = "CONFIGURACIÓN",
                ["language"] = "Idioma",
                ["next_check"] = "Próxima verificación en",
                ["seconds"] = "s",
                ["games"] = "juego(s)",
                ["cards_count"] = "carta(s)",
                ["stopped"] = "Detenido",
                ["stopped_manual"] = "El idling fue detenido manualmente.",
                ["finished"] = "¡Terminado! 🎉",
                ["priority_set"] = "establecido como prioritario.",
            },
            ["Deutsch"] = new()
            {
                ["auth"] = "AUTHENTIFIZIERUNG",
                ["connected"] = "✓ Verbunden",
                ["disconnect"] = "↩  Abmelden",
                ["connect"] = "🔑  Anmelden",
                ["options"] = "OPTIONEN",
                ["fast_mode"] = "Schnellmodus",
                ["fast_mode_desc"] = "Mehrere Spiele gleichzeitig idlen.",
                ["stats"] = "SITZUNGSSTATISTIK",
                ["cards_farmed"] = "Gesammelte Karten",
                ["games_done"] = "Abgeschlossene Spiele",
                ["session_time"] = "Sitzungszeit",
                ["refresh"] = "🔄  Aktualisieren",
                ["start"] = "▶  Starten",
                ["stop"] = "⏹  Stoppen",
                ["image"] = "BILD",
                ["game"] = "SPIEL",
                ["cards"] = "KARTEN",
                ["hours"] = "STUNDEN",
                ["prio"] = "PRIO",
                ["idling"] = "WIRD GEIDLET",
                ["connect_to_see"] = "Anmelden um Abzeichen zu sehen",
                ["ready"] = "Bereit",
                ["connect_to_start"] = "Anmelden zum Starten.",
                ["how_to_cookies"] = "Wie bekomme ich Cookies?\n1. Steam in Chrome öffnen\n2. F12 → Anwendung → Cookies\n3. steamcommunity.com\n4. steamLoginSecure und sessionid kopieren",
                ["settings"] = "EINSTELLUNGEN",
                ["language"] = "Sprache",
                ["next_check"] = "Nächste Prüfung in",
                ["seconds"] = "s",
                ["games"] = "Spiel(e)",
                ["cards_count"] = "Karte(n)",
                ["stopped"] = "Gestoppt",
                ["stopped_manual"] = "Idling wurde manuell gestoppt.",
                ["finished"] = "Fertig! 🎉",
                ["priority_set"] = "als Priorität gesetzt.",
            },
            ["Italiano"] = new()
            {
                ["auth"] = "AUTENTICAZIONE",
                ["connected"] = "✓ Connesso",
                ["disconnect"] = "↩  Disconnetti",
                ["connect"] = "🔑  Accedi",
                ["options"] = "OPZIONI",
                ["fast_mode"] = "Modalità Rapida",
                ["fast_mode_desc"] = "Fai idle su più giochi contemporaneamente.",
                ["stats"] = "STATISTICHE SESSIONE",
                ["cards_farmed"] = "Carte ottenute",
                ["games_done"] = "Giochi completati",
                ["session_time"] = "Durata sessione",
                ["refresh"] = "🔄  Aggiorna",
                ["start"] = "▶  Avvia",
                ["stop"] = "⏹  Ferma",
                ["image"] = "IMMAGINE",
                ["game"] = "GIOCO",
                ["cards"] = "CARTE",
                ["hours"] = "ORE",
                ["prio"] = "PRIO",
                ["idling"] = "IN ESECUZIONE",
                ["connect_to_see"] = "Accedi per vedere i tuoi badge",
                ["ready"] = "Pronto",
                ["connect_to_start"] = "Accedi per iniziare.",
                ["how_to_cookies"] = "Come ottenere i cookie?\n1. Apri Steam in Chrome\n2. F12 → Applicazione → Cookie\n3. steamcommunity.com\n4. Copia steamLoginSecure e sessionid",
                ["settings"] = "IMPOSTAZIONI",
                ["language"] = "Lingua",
                ["next_check"] = "Prossimo controllo tra",
                ["seconds"] = "s",
                ["games"] = "gioco/i",
                ["cards_count"] = "carta/e",
                ["stopped"] = "Fermato",
                ["stopped_manual"] = "L'idling è stato fermato manualmente.",
                ["finished"] = "Completato! 🎉",
                ["priority_set"] = "impostato come priorità.",
            },
            ["中文"] = new()
            {
                ["auth"] = "身份验证",
                ["connected"] = "✓ 已连接",
                ["disconnect"] = "↩  断开连接",
                ["connect"] = "🔑  登录",
                ["options"] = "选项",
                ["fast_mode"] = "快速模式",
                ["fast_mode_desc"] = "同时挂机多个游戏。",
                ["stats"] = "本次会话统计",
                ["cards_farmed"] = "已获得卡牌",
                ["games_done"] = "已完成游戏",
                ["session_time"] = "会话时间",
                ["refresh"] = "🔄  刷新",
                ["start"] = "▶  开始",
                ["stop"] = "⏹  停止",
                ["image"] = "图片",
                ["game"] = "游戏",
                ["cards"] = "卡牌",
                ["hours"] = "小时",
                ["prio"] = "优先",
                ["idling"] = "正在挂机",
                ["connect_to_see"] = "登录以查看您的徽章",
                ["ready"] = "就绪",
                ["connect_to_start"] = "登录以开始。",
                ["how_to_cookies"] = "如何获取Cookie？\n1. 在Chrome中打开Steam\n2. F12 → 应用程序 → Cookie\n3. steamcommunity.com\n4. 复制 steamLoginSecure 和 sessionid",
                ["settings"] = "设置",
                ["language"] = "语言",
                ["next_check"] = "下次检查于",
                ["seconds"] = "秒",
                ["games"] = "个游戏",
                ["cards_count"] = "张卡牌",
                ["stopped"] = "已停止",
                ["stopped_manual"] = "挂机已手动停止。",
                ["finished"] = "完成！🎉",
                ["priority_set"] = "已设为优先。",
            },
            ["Português"] = new()
            {
                ["auth"] = "AUTENTICAÇÃO",
                ["connected"] = "✓ Conectado",
                ["disconnect"] = "↩  Desconectar",
                ["connect"] = "🔑  Entrar",
                ["options"] = "OPÇÕES",
                ["fast_mode"] = "Modo Rápido",
                ["fast_mode_desc"] = "Idle em vários jogos ao mesmo tempo.",
                ["stats"] = "ESTATÍSTICAS DA SESSÃO",
                ["cards_farmed"] = "Cartas obtidas",
                ["games_done"] = "Jogos concluídos",
                ["session_time"] = "Tempo de sessão",
                ["refresh"] = "🔄  Atualizar",
                ["start"] = "▶  Iniciar",
                ["stop"] = "⏹  Parar",
                ["image"] = "IMAGEM",
                ["game"] = "JOGO",
                ["cards"] = "CARTAS",
                ["hours"] = "HORAS",
                ["prio"] = "PRIO",
                ["idling"] = "EM EXECUÇÃO",
                ["connect_to_see"] = "Entre para ver seus emblemas",
                ["ready"] = "Pronto",
                ["connect_to_start"] = "Entre para começar.",
                ["how_to_cookies"] = "Como obter os cookies?\n1. Abra o Steam no Chrome\n2. F12 → Aplicativo → Cookies\n3. steamcommunity.com\n4. Copie steamLoginSecure e sessionid",
                ["settings"] = "CONFIGURAÇÕES",
                ["language"] = "Idioma",
                ["next_check"] = "Próxima verificação em",
                ["seconds"] = "s",
                ["games"] = "jogo(s)",
                ["cards_count"] = "carta(s)",
                ["stopped"] = "Parado",
                ["stopped_manual"] = "O idling foi parado manualmente.",
                ["finished"] = "Concluído! 🎉",
                ["priority_set"] = "definido como prioridade.",
            },
            ["日本語"] = new()
            {
                ["auth"] = "認証",
                ["connected"] = "✓ 接続済み",
                ["disconnect"] = "↩  ログアウト",
                ["connect"] = "🔑  ログイン",
                ["options"] = "オプション",
                ["fast_mode"] = "高速モード",
                ["fast_mode_desc"] = "複数のゲームを同時にアイドル。",
                ["stats"] = "セッション統計",
                ["cards_farmed"] = "取得カード数",
                ["games_done"] = "完了ゲーム数",
                ["session_time"] = "セッション時間",
                ["refresh"] = "🔄  更新",
                ["start"] = "▶  開始",
                ["stop"] = "⏹  停止",
                ["image"] = "画像",
                ["game"] = "ゲーム",
                ["cards"] = "カード",
                ["hours"] = "時間",
                ["prio"] = "優先",
                ["idling"] = "アイドル中",
                ["connect_to_see"] = "ログインしてバッジを確認",
                ["ready"] = "準備完了",
                ["connect_to_start"] = "ログインして開始してください。",
                ["how_to_cookies"] = "Cookieの取得方法\n1. ChromeでSteamを開く\n2. F12 → アプリケーション → Cookie\n3. steamcommunity.com\n4. steamLoginSecureとsessionidをコピー",
                ["settings"] = "設定",
                ["language"] = "言語",
                ["next_check"] = "次のチェックまで",
                ["seconds"] = "秒",
                ["games"] = "ゲーム",
                ["cards_count"] = "カード",
                ["stopped"] = "停止",
                ["stopped_manual"] = "アイドルが手動で停止されました。",
                ["finished"] = "完了！🎉",
                ["priority_set"] = "を優先に設定しました。",
            },
        };

        public string Get(string key)
        {
            if (_translations.TryGetValue(_currentLanguage, out var dict) && dict.TryGetValue(key, out var val))
                return val;
            if (_translations.TryGetValue("Français", out var fallback) && fallback.TryGetValue(key, out var fb))
                return fb;
            return key;
        }

        // Propriétés exposées pour le binding XAML
        public string Auth => Get("auth");
        public string Connected => Get("connected");
        public string Disconnect => Get("disconnect");
        public string Connect => Get("connect");
        public string Options => Get("options");
        public string FastMode => Get("fast_mode");
        public string FastModeDesc => Get("fast_mode_desc");
        public string Stats => Get("stats");
        public string CardsFarmed => Get("cards_farmed");
        public string GamesDone => Get("games_done");
        public string SessionTime => Get("session_time");
        public string Refresh => Get("refresh");
        public string Start => Get("start");
        public string Stop => Get("stop");
        public string ImageCol => Get("image");
        public string GameCol => Get("game");
        public string CardsCol => Get("cards");
        public string HoursCol => Get("hours");
        public string PrioCol => Get("prio");
        public string Idling => Get("idling");
        public string ConnectToSee => Get("connect_to_see");
        public string Ready => Get("ready");
        public string ConnectToStart => Get("connect_to_start");
        public string HowToCookies => Get("how_to_cookies");
        public string Settings => Get("settings");
        public string Language => Get("language");
        public string NextCheck => Get("next_check");
        public string Seconds => Get("seconds");
        public string Games => Get("games");
        public string CardsCount => Get("cards_count");
        public string Stopped => Get("stopped");
        public string StoppedManual => Get("stopped_manual");
        public string Finished => Get("finished");

        private void OnAllChanged()
        {
            var props = GetType().GetProperties().Select(p => p.Name).ToArray();
            foreach (var p in props) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
