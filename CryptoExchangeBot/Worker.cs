using CryptoExchangeBot.Data;
using CryptoExchangeBot.Helpers;
using CryptoExchangeBot.Models.Entities;
using CryptoExchangeBot.Models.Settings;
using CryptoExchangeBot.Quartz;
using CryptoExchangeBot.Quartz.Jobs;
using Google.Apis.Sheets.v4;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Serilog;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = CryptoExchangeBot.Models.Entities.User;
using Serilog;
using ILogger = Serilog.ILogger;

namespace CryptoExchangeBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;

        private readonly TextSettings _textSettings;
        private readonly TelegramBotSettings _telegramBotSettings;
        private readonly QuartzSettings _quartzSettings;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public Worker(ILogger logger, IOptions<TextSettings> textSettingsOpt, IOptions<TelegramBotSettings> telegramBotSettingsOpt, 
            IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IOptions<QuartzSettings> quartzSettingsOpt)
        {
            _logger = logger;

            _textSettings = textSettingsOpt.Value;
            _telegramBotSettings = telegramBotSettingsOpt.Value;
            _quartzSettings = quartzSettingsOpt.Value;

            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceScopeFactory.CreateScope();
            await DataScheduler.Initialize(scope.ServiceProvider, _quartzSettings, stoppingToken);
            Console.WriteLine("Quartz запущен");

            var botClient = new TelegramBotClient(_telegramBotSettings.Token);
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: stoppingToken);
            Console.WriteLine("Бот запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Error("Executed");
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                using var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        return;
                    }

                    if (message.Text.Equals("/start", StringComparison.OrdinalIgnoreCase))
                    {
                        await HandleStartMessages(botClient, message);
                        return;
                    }

                    if (message.Text.Equals(_textSettings.ButtonWantStudy, StringComparison.OrdinalIgnoreCase) && !context.Users.Any(x => x.ChatId == message.Chat.Id))
                    {
                        await HandleReadyMessage(botClient, context, message);
                        return;
                    }

                    var user = context.Users.First(x => x.ChatId == message.Chat.Id);
                    if (string.IsNullOrEmpty(user.Name))
                    {
                        await HandleNameMessage(botClient, context, message, user);
                        return;
                    }

                    if (string.IsNullOrEmpty(user.Phone))
                    {
                        await HandlePhoneMessage(botClient, context, message, user);
                        return;
                    }

                    if (string.IsNullOrEmpty(user.Nickname))
                    {
                        await HandleNicknameMessage(botClient, context, message, user);
                        return;
                    }

                    var dailyMessageExecutedDate = await DataScheduler.GetDailyEveningTriggerPreviousFireTime();
                    var dailyEarning = await context.DailyEarnings.FirstOrDefaultAsync(x => x.ChatId == message.Chat.Id && x.DateCreated.Date == DateTime.Today);
                    if (dailyEarning == default && message.Text.Equals(_textSettings.ButtonShare) && DateTime.Now.Date == dailyMessageExecutedDate.Date)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, _textSettings.DailyCommentTimeExpired, replyMarkup: new ReplyKeyboardRemove());
                        return;
                    }
                    
                    if (dailyEarning == default && message.Text.Equals(_textSettings.ButtonShare))
                    {
                        await HandleShareMessage(botClient, context, message);
                        return;
                    }

                    if (dailyEarning != default && !dailyEarning.Amount.HasValue)
                    {
                        await HandleDailyAmountMessage(botClient, context, dailyEarning, message);
                        return;
                    }

                    if (dailyEarning != default && dailyEarning.Amount.HasValue)
                    {
                        await HandleDailyCommentMessage(botClient, context, dailyEarning, message);
                        return;
                    }

                    await botClient.SendTextMessageAsync(message.Chat, _textSettings.IncorrectResponse, replyMarkup: new ReplyKeyboardRemove());
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error(exception, exception.Message);
        }

        //Обрабатывает /start
        private async Task HandleStartMessages(ITelegramBotClient botClient, Message message)
        {
            foreach (var greeting in _textSettings.Greeting)
            {
                if (_textSettings.Greeting.Last() == greeting)
                {
                    var startButton = TgBotHelper.GetButton(_textSettings.ButtonWantStudy);
                    await botClient.SendTextMessageAsync(message.Chat, greeting, ParseMode.Html, replyMarkup: startButton);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, greeting, replyMarkup: new ReplyKeyboardRemove());
                }
            }
        }

        //Обрабатывает нажатие на кнопку, хочу учиться
        private async Task HandleReadyMessage(ITelegramBotClient botClient, ApplicationContext context, Message message)
        {
            await context.Users.AddAsync(new User()
            {
                ChatId = message.Chat.Id,
            });
            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(message.Chat, _textSettings.NameRequest, replyMarkup: new ReplyKeyboardRemove());
        }

        //Обрабатывает ввод имени
        private async Task HandleNameMessage(ITelegramBotClient botClient, ApplicationContext context, Message message, User user)
        {
            user.Name = message.Text;
            context.Users.Update(user);
            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(message.Chat, _textSettings.PhoneRequest, replyMarkup: new ReplyKeyboardRemove());
        }

        //Обрабатывает ввод телефона
        private async Task HandlePhoneMessage(ITelegramBotClient botClient, ApplicationContext context, Message message, User user)
        {
            user.Phone = message.Text;
            context.Users.Update(user);
            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(message.Chat, _textSettings.NicknameRequest, replyMarkup: new ReplyKeyboardRemove());
        }

        //Обрабатывает ввод никнейма
        private async Task HandleNicknameMessage(ITelegramBotClient botClient, ApplicationContext context, Message message, User user)
        {
            user.Nickname = message.Text;
            context.Users.Update(user);
            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(message.Chat, _textSettings.DataSuccessfullyAdded, replyMarkup: new ReplyKeyboardRemove());
        }

        //Обрабатывает нажатие кнопки Поделится
        private async Task HandleShareMessage(ITelegramBotClient botClient, ApplicationContext context, Message message)
        {
            DailyEarning dailyEarning = new()
            {
                ChatId = message.Chat.Id,
                DateCreated = DateTime.Now
            };
            await context.DailyEarnings.AddAsync(dailyEarning);
            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(message.Chat, _textSettings.DailyAmountRequest, replyMarkup: new ReplyKeyboardRemove());
        }

        //Обрабатывает ввод суммы за день
        private async Task HandleDailyAmountMessage(ITelegramBotClient botClient, ApplicationContext context, DailyEarning dailyEarning, Message message)
        {
            if (double.TryParse(message.Text, out var amount))
            {
                dailyEarning.Amount = amount;

                context.DailyEarnings.Update(dailyEarning);
                await context.SaveChangesAsync();

                await botClient.SendTextMessageAsync(message.Chat, _textSettings.DailyCommentRequest, replyMarkup: new ReplyKeyboardRemove());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat, _textSettings.IncorrectAmountResponse, replyMarkup: new ReplyKeyboardRemove());
                await botClient.SendTextMessageAsync(message.Chat, _textSettings.DailyAmountRequest, replyMarkup: new ReplyKeyboardRemove());
            }
        }

        //Обрабатывает ввод комментария за день
        private async Task HandleDailyCommentMessage(ITelegramBotClient botClient, ApplicationContext context, DailyEarning dailyEarning, Message message)
        {
            if (string.IsNullOrEmpty(dailyEarning.Comment))
            {
                dailyEarning.Comment = message.Text;
                context.DailyEarnings.Update(dailyEarning);
                await context.SaveChangesAsync();

                await botClient.SendTextMessageAsync(message.Chat, _textSettings.DailyDateSucesfullyAdded, replyMarkup: new ReplyKeyboardRemove());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat, _textSettings.DailyEarningAlreadyExists, replyMarkup: new ReplyKeyboardRemove());
            }
        }
    }
}