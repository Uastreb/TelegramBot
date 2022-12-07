using CryptoExchangeBot.Data;
using CryptoExchangeBot.Models.Settings;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;
using Telegram.Bot.Types.ReplyMarkups;

namespace CryptoExchangeBot.Quartz.Jobs
{
    public class SendFinishedMessageJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        private readonly TextSettings _textSettings;

        public SendFinishedMessageJob(IServiceScopeFactory serviceScopeFactory, ITelegramBotClient telegramBotClient,
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

                foreach (var user in dbContext.Users)
                {
                    try
                    {
                        var points = await dbContext.DailyEarnings.CountAsync(x => x.ChatId == user.ChatId && x.Amount.HasValue);

                        await _botClient.SendTextMessageAsync(new ChatId(user.ChatId), string.Format(_textSettings.FinishedMesage, points), replyMarkup: new ReplyKeyboardRemove());
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, ex.Message);
                    }
                }
            }
        }
    }
}
