namespace TelegramClient.KeyWords
{
    public class Settings
    {
        public string[] KeyWords { get; set; }
        public long? IdChatToPaste { get; set; }

        public string ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; } // if sign-up is required
        public string LastName { get; set; }  // if sign-up is required
        public string Password { get; set; } // if user has enabled 2FA
    }
}
