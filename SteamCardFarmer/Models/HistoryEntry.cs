namespace SteamCardFarmer.Models
{
    public class HistoryEntry
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public string GameName { get; set; } = "";
        public int AppId { get; set; }
        public int CardsDropped { get; set; }

        public string DateText => Date.ToString("dd/MM/yyyy HH:mm");
        public string Summary => $"+{CardsDropped} 🃏  {GameName}";
    }
}
