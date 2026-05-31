using System.IO;
using System.Text.Json;
using SteamCardFarmer.Models;

namespace SteamCardFarmer.Services
{
    public static class HistoryService
    {
        private static readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Unlock Steam Cards", "history.json");

        public static List<HistoryEntry> Load()
        {
            try
            {
                if (!File.Exists(_path)) return new();
                var json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<List<HistoryEntry>>(json) ?? new();
            }
            catch { return new(); }
        }

        public static void Add(HistoryEntry entry)
        {
            try
            {
                var list = Load();
                list.Insert(0, entry); // plus récent en premier
                if (list.Count > 500) list = list.Take(500).ToList(); // limite 500 entrées

                Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                File.WriteAllText(_path, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        public static void Clear()
        {
            try { if (File.Exists(_path)) File.Delete(_path); } catch { }
        }
    }
}
