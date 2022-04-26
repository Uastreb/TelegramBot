namespace TelegramBot.KeyWords.Models
{
    public class Settings
    {
        public long AdminChatId { get; set; }
        public string Token { get; set; }
        public string[] KeyWords { get; set; }
    }
}
