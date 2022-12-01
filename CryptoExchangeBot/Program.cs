using CryptoExchangeBot.Data;
using CryptoExchangeBot.Models.Settings;
using CryptoExchangeBot.Models.Settings.GoogleSheets;
using CryptoExchangeBot.Quartz;
using CryptoExchangeBot.Quartz.Jobs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Telegram.Bot;

namespace CryptoExchangeBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "CryptoExchangeBot";
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddDbContext<ApplicationContext>(opts => opts.UseSqlite("Data Source=CryptoExchange.db"));

                    AddTextSettings(services, hostContext);
                    AddTelegramBot(services, hostContext);
                    AddQuartz(services, hostContext);
                    AddGoogleSheetsService(services, hostContext);
                });

            AddSerilog(hostBuilder);

            var host = hostBuilder.Build();

            host.Run();
        }

        private static IServiceCollection AddTextSettings(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            IConfiguration configuration = hostBuilderContext.Configuration;
            var textSettingsConfiguration = configuration.GetSection(nameof(TextSettings));
            services.Configure<TextSettings>(textSettingsConfiguration);

            return services;
        }

        private static IServiceCollection AddTelegramBot(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            IConfiguration configuration = hostBuilderContext.Configuration;
            var telegramBotSettingsConfiguration = configuration.GetSection(nameof(TelegramBotSettings));

            services.Configure<TelegramBotSettings>(telegramBotSettingsConfiguration);
            var telegramBotSettings = telegramBotSettingsConfiguration.Get<TelegramBotSettings>();
            services.AddSingleton<ITelegramBotClient, TelegramBotClient>(opt => new TelegramBotClient(telegramBotSettings.Token));

            return services;
        }

        private static IHostBuilder AddSerilog(IHostBuilder builder)
        {
            builder.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

            return builder;
        }

        private static IServiceCollection AddQuartz(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            IConfiguration configuration = hostBuilderContext.Configuration;
            var quartzSettingsConfiguration = configuration.GetSection(nameof(QuartzSettings));

            services.Configure<QuartzSettings>(quartzSettingsConfiguration);
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });

            services.AddScoped<JobFactory>();
            services.AddScoped<SendDailyEveningMessageJob>();
            services.AddScoped<SendDailyMorningMessageJob>();
            services.AddScoped<SendFinishedMessageJob>();
            services.AddScoped<UpdateUserResultsJob>();

            return services;
        }

        private static IServiceCollection AddGoogleSheetsService(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            IConfiguration configuration = hostBuilderContext.Configuration;
            var googleSheetsConfiguration = configuration.GetSection(nameof(GoogleSheetsConfiguration));

            services.Configure<GoogleSheetsConfiguration>(googleSheetsConfiguration);
            var googleSheetsConf = googleSheetsConfiguration.Get<GoogleSheetsConfiguration>();

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy 
            var credential = GoogleCredential.FromJson(googleSheetsConf.SettingsJSON.GetJson());

            services.AddScoped<SheetsService>(x => new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = googleSheetsConf.ApplicationName
            }));

            return services;
        }
    }
}