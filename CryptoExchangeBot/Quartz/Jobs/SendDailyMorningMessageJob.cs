using CryptoExchangeBot.Data;
using CryptoExchangeBot.Helpers;
using CryptoExchangeBot.Models.Settings;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types;
using ILogger = Serilog.ILogger;

namespace CryptoExchangeBot.Quartz.Jobs
{
    public class SendDailyMorningMessageJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        private readonly TextSettings _textSettings;

        public SendDailyMorningMessageJob(IServiceScopeFactory serviceScopeFactory, ITelegramBotClient telegramBotClient,
            IOptions<TextSettings> textSettingsOpt, ILogger logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _botClient = telegramBotClient;
            _logger = logger;

            _textSettings = textSettingsOpt.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationContext>();

                foreach (var user in dbContext.Users.Where(x => x.Name != null && x.Phone != null && x.Nickname != null))
                {
                    try
                    {
                        var button = TgBotHelper.GetButton(_textSettings.ButtonShare);
                        await _botClient.SendTextMessageAsync(new ChatId(user.ChatId), _textSettings.DailyMorningMessage, replyMarkup: button);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, ex.Message);
                    }
                }
            }
        }
    }
}
