using System.IO;

namespace SteamCardFarmer.Services
{
    /// <summary>
    /// Trouve et copie automatiquement steam_api64.dll depuis les jeux Steam installés.
    /// </summary>
    public static class SteamApiHelper
    {
        public static bool IsReady(string appDir)
            => File.Exists(Path.Combine(appDir, "steam_api64.dll"));

        public static string? FindAndCopy(string appDir)
        {
            var destination = Path.Combine(appDir, "steam_api64.dll");
            if (File.Exists(destination)) return null; // déjà là

            // Cherche dans les emplacements Steam courants
            var steamPaths = GetSteamLibraryPaths();

            foreach (var steamLib in steamPaths)
            {
                var commonDir = Path.Combine(steamLib, "steamapps", "common");
                if (!Directory.Exists(commonDir)) continue;

                foreach (var gameDir in Directory.EnumerateDirectories(commonDir))
                {
                    var dll = Path.Combine(gameDir, "steam_api64.dll");
                    if (File.Exists(dll))
                    {
                        try
                        {
                            File.Copy(dll, destination, overwrite: true);
                            return gameDir; // succès — retourne le jeu source
                        }
                        catch { }
                    }
                }
            }

            return null; // non trouvé
        }

        private static List<string> GetSteamLibraryPaths()
        {
            var paths = new List<string>();

            // 1. Registre Windows — trouve Steam peu importe le disque
            var steamRoot = GetSteamPathFromRegistry();
            if (!string.IsNullOrEmpty(steamRoot) && Directory.Exists(steamRoot))
                paths.Add(steamRoot);

            // 2. Emplacements par défaut en fallback
            var defaults = new[]
            {
                @"C:\Program Files (x86)\Steam",
                @"C:\Program Files\Steam",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
            };
            foreach (var d in defaults.Where(Directory.Exists))
                if (!paths.Contains(d)) paths.Add(d);

            // 3. Lit libraryfolders.vdf pour les bibliothèques sur d'autres disques
            foreach (var root in paths.ToList())
            {
                var vdf = Path.Combine(root, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(vdf)) continue;

                foreach (var line in File.ReadAllLines(vdf))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"""path""\s+""(.+?)""");
                    if (match.Success)
                    {
                        var p = match.Groups[1].Value.Replace(@"\\", @"\");
                        if (Directory.Exists(p) && !paths.Contains(p))
                            paths.Add(p);
                    }
                }
            }

            return paths;
        }

        private static string? GetSteamPathFromRegistry()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser
                    .OpenSubKey(@"Software\Valve\Steam");
                return key?.GetValue("SteamPath")?.ToString()?.Replace('/', '\\');
            }
            catch { }

            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine
                    .OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
                return key?.GetValue("InstallPath")?.ToString();
            }
            catch { }

            return null;
        }
    }
}
