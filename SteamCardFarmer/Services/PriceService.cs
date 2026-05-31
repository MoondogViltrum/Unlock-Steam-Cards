using System.Net.Http;
using System.Text.Json;

namespace SteamCardFarmer.Services
{
    public static class PriceService
    {
        private static readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(10) };

        // Cache pour éviter trop d'appels API
        private static readonly Dictionary<int, double> _cache = new();

        /// <summary>
        /// Retourne le prix moyen estimé des cartes d'un jeu en euros.
        /// Utilise l'API steamcardexchange.net
        /// </summary>
        public static async Task<double> GetAverageCardPriceAsync(int appId)
        {
            if (_cache.TryGetValue(appId, out var cached)) return cached;

            try
            {
                var url = $"https://www.steamcardexchange.net/api/request.php?GetBadgeInfo&badge={appId}";
                var json = await _client.GetStringAsync(url);
                var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("cards", out var cards))
                {
                    var prices = new List<double>();
                    foreach (var card in cards.EnumerateArray())
                    {
                        if (card.TryGetProperty("price", out var price))
                        {
                            var val = price.GetDouble() / 100.0; // en centimes → euros
                            if (val > 0) prices.Add(val);
                        }
                    }

                    if (prices.Any())
                    {
                        var avg = prices.Average();
                        _cache[appId] = avg;
                        return avg;
                    }
                }
            }
            catch { }

            _cache[appId] = 0;
            return 0;
        }

        public static void ClearCache() => _cache.Clear();
    }
}
