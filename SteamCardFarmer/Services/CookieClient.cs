using System.Net;
using System.Net.Http;

namespace SteamCardFarmer.Services
{
    /// <summary>
    /// HTTP client that sends Steam cookies for authenticated requests.
    /// </summary>
    public static class CookieClient
    {
        private static HttpClient? _client;

        public static void Initialize(string steamLoginSecure, string sessionId)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AllowAutoRedirect = true
            };

            var uri = new Uri("https://steamcommunity.com");
            handler.CookieContainer.Add(uri, new Cookie("steamLoginSecure", steamLoginSecure) { Secure = true });
            handler.CookieContainer.Add(uri, new Cookie("sessionid", sessionId));
            handler.CookieContainer.Add(uri, new Cookie("Steam_Language", "french"));

            _client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public static async Task<string> GetAsync(string url)
        {
            if (_client == null) throw new InvalidOperationException("CookieClient not initialized.");
            var response = await _client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public static bool IsInitialized => _client != null;

        public static void Reset() => _client = null;
    }
}
