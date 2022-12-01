namespace CryptoExchangeBot.Models.Settings
{
    public class QuartzSettings
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string ExecutingMorningTime { get; set; }
        public string ExecutingEveningTime { get; set; }
        public DateTime FinishedMessageDate { get; set; }
        public string ExecutingUpdateUserResultsTime { get; set; }
    }
}
