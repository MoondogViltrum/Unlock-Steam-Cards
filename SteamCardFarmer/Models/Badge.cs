using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SteamCardFarmer.Models
{
    public class Badge
    {
        public int AppId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RemainingCards { get; set; }
        public double HoursPlayed { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        private Process? _idleProcess;
        public bool IsIdling => _idleProcess != null && !_idleProcess.HasExited;

        public string StringId => AppId.ToString();

        public string RemainingCardsText => $"{RemainingCards} 🃏";
        public string HoursPlayedText => $"{HoursPlayed:F1}h";

        public void StartIdle(string steamIdlePath)
        {
            if (IsIdling) return;
            if (!System.IO.File.Exists(steamIdlePath))
                throw new System.IO.FileNotFoundException(
                    $"steam-idle.exe introuvable ici :\n{System.IO.Path.GetDirectoryName(steamIdlePath)}");

            var workingDir = System.IO.Path.GetDirectoryName(steamIdlePath)!;

            // Écrit steam_appid.txt dans le dossier de steam-idle.exe (requis par Steamworks)
            System.IO.File.WriteAllText(System.IO.Path.Combine(workingDir, "steam_appid.txt"), AppId.ToString());

            _idleProcess = Process.Start(new ProcessStartInfo(steamIdlePath, AppId.ToString())
            {
                // Fenêtre visible pour voir les erreurs de démarrage
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                WorkingDirectory = workingDir,
                UseShellExecute = false
            });

            // Vérifie si le processus a crashé immédiatement
            System.Threading.Thread.Sleep(500);
            if (_idleProcess == null || _idleProcess.HasExited)
            {
                _idleProcess = null;
                throw new Exception(
                    $"steam-idle.exe a crashé immédiatement.\n" +
                    $"Vérifie que steam_api.dll est dans :\n{workingDir}");
            }
        }

        public void StopIdle()
        {
            if (_idleProcess == null || _idleProcess.HasExited) return;
            try { _idleProcess.Kill(); } catch { }
            _idleProcess = null;
        }

        public void UpdateStats(string? remaining, string? hours)
        {
            RemainingCards = string.IsNullOrWhiteSpace(remaining) ? 0 : int.Parse(remaining);
            HoursPlayed = string.IsNullOrWhiteSpace(hours) ? 0 : double.Parse(hours, new NumberFormatInfo());
        }

        public override string ToString() => string.IsNullOrWhiteSpace(Name) ? StringId : Name;
    }
}
