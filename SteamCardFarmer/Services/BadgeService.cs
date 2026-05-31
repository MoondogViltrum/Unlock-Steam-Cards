using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using SteamCardFarmer.Models;

namespace SteamCardFarmer.Services
{
    /// <summary>
    /// Scrapes the Steam badge pages to get games with remaining card drops.
    /// </summary>
    public static class BadgeService
    {
        public static async Task<List<Badge>> GetBadgesAsync(string profileUrl)
        {
            var badges = new List<Badge>();
            int page = 1;

            while (true)
            {
                var url = $"{profileUrl}/badges/?p={page}&sort=p";
                var html = await CookieClient.GetAsync(url);
                if (string.IsNullOrEmpty(html)) break;

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var badgeNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'badge_row')]");
                if (badgeNodes == null) break;

                bool foundAny = false;
                foreach (var node in badgeNodes)
                {
                    var badge = ParseBadge(node);
                    if (badge != null)
                    {
                        badges.Add(badge);
                        foundAny = true;
                    }
                }

                // Check if there's a next page
                var nextPage = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'pagebtn') and text()='>']");
                if (nextPage == null || !foundAny) break;
                page++;
            }

            return badges.OrderBy(b => b.HoursPlayed).ThenByDescending(b => b.RemainingCards).ToList();
        }

        private static Badge? ParseBadge(HtmlNode node)
        {
            try
            {
                // Ignore family-shared games (class badge_row_overlay ou mention "family sharing")
                var rowClass = node.GetAttributeValue("class", "");
                if (rowClass.Contains("family_sharing")) return null;

                // Ignore friend activity rows
                if (node.SelectSingleNode(".//div[contains(@class,'badge_row_overlay')]") != null)
                {
                    var overlay = node.SelectSingleNode(".//div[contains(@class,'badge_row_overlay')]");
                    if (overlay?.InnerText.Contains("famille", StringComparison.OrdinalIgnoreCase) == true
                     || overlay?.InnerText.Contains("family", StringComparison.OrdinalIgnoreCase) == true)
                        return null;
                }

                // Get AppId from link
                var linkNode = node.SelectSingleNode(".//a[contains(@href,'gamecards')]");
                if (linkNode == null) return null;

                var href = linkNode.GetAttributeValue("href", "");
                var appIdMatch = Regex.Match(href, @"/gamecards/(\d+)");
                if (!appIdMatch.Success) return null;
                var appId = int.Parse(appIdMatch.Groups[1].Value);

                // Get remaining cards
                var progressNode = node.SelectSingleNode(".//span[contains(@class,'progress_info_bold')]");
                if (progressNode == null) return null;

                var cardsText = Regex.Match(progressNode.InnerText, @"\d+").Value;
                if (string.IsNullOrEmpty(cardsText)) return null;
                var remaining = int.Parse(cardsText);
                if (remaining == 0) return null;

                // Get game name — plusieurs tentatives
                var name = $"App {appId}";

                // Tentative 1 : texte direct du badge_title
                var nameNode = node.SelectSingleNode(".//div[contains(@class,'badge_title')]");
                if (nameNode != null)
                {
                    var rawText = nameNode.ChildNodes
                        .Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                        .Select(n => n.InnerText.Trim())
                        .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                    if (!string.IsNullOrWhiteSpace(rawText))
                        name = System.Net.WebUtility.HtmlDecode(rawText);
                }

                // Tentative 2 : via le lien du jeu
                if (name == $"App {appId}")
                {
                    var titleLink = node.SelectSingleNode(".//a[contains(@href,'gamecards')]");
                    if (titleLink != null)
                    {
                        var t = titleLink.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(t) && t != "View Badge")
                            name = System.Net.WebUtility.HtmlDecode(t);
                    }
                }

                // Tentative 3 : nettoyage du InnerText complet du badge_title
                if (name == $"App {appId}" && nameNode != null)
                {
                    var full = nameNode.InnerText.Trim();
                    var firstLine = full.Split('\n', '\r').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim();
                    if (!string.IsNullOrWhiteSpace(firstLine))
                        name = System.Net.WebUtility.HtmlDecode(firstLine);
                }

                // Get hours played
                var hoursNode = node.SelectSingleNode(".//div[contains(@class,'badge_title_stats_playtime')]");
                var hours = 0.0;
                if (hoursNode != null)
                {
                    var hoursMatch = Regex.Match(hoursNode.InnerText, @"[\d,\.]+");
                    if (hoursMatch.Success)
                        double.TryParse(hoursMatch.Value.Replace(",", "."), System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out hours);
                }

                return new Badge
                {
                    AppId = appId,
                    Name = name,
                    RemainingCards = remaining,
                    HoursPlayed = hours,
                    ImageUrl = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{appId}/header.jpg"
                };
            }
            catch
            {
                return null;
            }
        }

        public static async Task<int> GetRemainingCardsForGame(string profileUrl, int appId)
        {
            try
            {
                var html = await CookieClient.GetAsync($"{profileUrl}/gamecards/{appId}");
                if (string.IsNullOrEmpty(html)) return -1;

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var progressNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'progress_info_bold')]");
                if (progressNode == null) return 0;

                var match = Regex.Match(progressNode.InnerText, @"\d+");
                return match.Success ? int.Parse(match.Value) : 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
