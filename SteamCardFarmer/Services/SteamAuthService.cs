using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace SteamCardFarmer.Services
{
    public class SteamAuthResult
    {
        public bool Success { get; set; }
        public string SteamId { get; set; } = "";
        public string ProfileUrl { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Error { get; set; } = "";
    }

    /// <summary>
    /// Handles Steam authentication via cookies extracted from the browser
    /// or stored in app settings.
    /// </summary>
    public static class SteamAuthService
    {
        public static bool IsSteamRunning()
        {
            return System.Diagnostics.Process.GetProcessesByName("steam").Length > 0;
        }

        public static string ExtractSteamId(string steamLoginSecure)
        {
            var decoded = WebUtility.UrlDecode(steamLoginSecure);
            var index = decoded.IndexOf('|');
            return index != -1 ? decoded[..index] : decoded;
        }

        public static async Task<SteamAuthResult> AuthenticateAsync(string steamLoginSecure, string sessionId)
        {
            try
            {
                var steamId = ExtractSteamId(steamLoginSecure);
                var profileUrl = $"https://steamcommunity.com/profiles/{steamId}";

                CookieClient.Initialize(steamLoginSecure, sessionId);

                // Verify authentication by fetching profile
                var html = await CookieClient.GetAsync($"{profileUrl}/badges/");
                if (string.IsNullOrEmpty(html) || html.Contains("g_steamID = false"))
                {
                    return new SteamAuthResult { Error = "Cookies invalides ou expirés." };
                }

                // Get username from Steam XML API
                var userName = await GetUserNameAsync(profileUrl, steamId);

                return new SteamAuthResult
                {
                    Success = true,
                    SteamId = steamId,
                    ProfileUrl = profileUrl,
                    UserName = userName
                };
            }
            catch (Exception ex)
            {
                return new SteamAuthResult { Error = ex.Message };
            }
        }

        private static async Task<string> GetUserNameAsync(string profileUrl, string steamId)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "SteamCardFarmer/1.0");
                var xml = await client.GetStringAsync($"{profileUrl}/?xml=1");
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var nameNode = doc.SelectSingleNode("//steamID");
                return nameNode != null ? WebUtility.HtmlDecode(nameNode.InnerText) : $"User {steamId}";
            }
            catch
            {
                return $"User {steamId}";
            }
        }
    }
}
