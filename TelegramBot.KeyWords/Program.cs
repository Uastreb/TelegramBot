using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.KeyWords.Models;

namespace TelegramBot.KeyWords
{
    class Program
    {
        private static Settings _settings;
        private static ITelegramBotClient _telegramBotClient;

        private static string _token = string.Empty;

        private static ReceiverOptions _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };
        private static CancellationTokenSource _cts = new CancellationTokenSource();


        static void Main(string[] args)
        {
            try
            {
                _settings = XmlSerializationHelper<Settings>.DeserializeFromFile(@"Settings.xml");
                _token = _settings.Token;
                _telegramBotClient = new TelegramBotClient(_token);
                _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, _receiverOptions, cancellationToken: _cts.Token);

                Console.WriteLine("Введите Cancel для прекращения работы и выхода");
                var text = string.Empty;
                do
                {
                    text = Console.ReadLine();
                }
                while (!string.Equals(text, "Cancel", StringComparison.OrdinalIgnoreCase));

                // Send cancellation request to stop bot
                _cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Проиошла ошибка{Environment.NewLine}Проверьте правильно ли настроен файл настроек{Environment.NewLine}{Environment.NewLine}{ex}");
                Console.ReadLine();
            }
        }


        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            await HandleMessage(update.Message);
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);

            return Task.CompletedTask;
        }

        private static async Task HandleMessage(Message message)
        {
            if (message.Text == "Hi i'm a admin")
            {
                _settings.AdminChatId = message.Chat.Id;

                XmlSerializationHelper<Settings>.SerializeInFile(_settings, "Settings.xml");

                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Hi admin");

                return;
            }

            foreach (var keyWord in _settings.KeyWords)
            {
                if (message.Text != default && message.Text.Contains(keyWord))
                {
                    await _telegramBotClient.ForwardMessageAsync(_settings.AdminChatId, message.Chat.Id, message.MessageId);
                    //await _telegramBotClient.SendTextMessageAsync(_settings.AdminChatId, message.);

                    return;
                }
            }
        }
    }
}
