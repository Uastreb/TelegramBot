using Telegram.Bot.Types.ReplyMarkups;

namespace CryptoExchangeBot.Helpers
{
    public static class TgBotHelper
    {
        public static IReplyMarkup GetButton(string text)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { text },
            })
            {
                ResizeKeyboard = true
            };

            return replyKeyboardMarkup;
        }
    }
}
