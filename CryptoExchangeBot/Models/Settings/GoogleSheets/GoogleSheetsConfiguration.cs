namespace CryptoExchangeBot.Models.Settings.GoogleSheets
{
    public class GoogleSheetsConfiguration
    {
        public string ApplicationName { get; set; }
        public string SpeadsheetId { get; set; }
        public GoogleSheetsSettings SettingsJSON { get; set; }
    }
}