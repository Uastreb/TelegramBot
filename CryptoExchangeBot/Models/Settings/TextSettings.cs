namespace CryptoExchangeBot.Models.Settings
{
    public class TextSettings
    {
        public string[] Greeting { get; set; }

        public string PhoneRequest { get; set; }
        public string NameRequest { get; set; }
        public string NicknameRequest { get; set; }
        public string DataSuccessfullyAdded { get; set; }

        public string DailyMorningMessage { get; set; }
        public string DailyEveningMessage { get; set; }

        public string DailyAmountRequest { get; set; }
        public string DailyCommentRequest { get; set; }
        public string DailyEarningAlreadyExists { get; set; }
        public string DailyDateSucesfullyAdded { get; set; }

        public string FinishedMesage { get; set; }

        public string ButtonWantStudy { get; set; }
        public string ButtonShare { get; set; }

        public string IncorrectAmountResponse { get; set; }
        public string IncorrectResponse { get; set; }
    }
}
