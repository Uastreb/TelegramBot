using Quartz.Impl;
using Quartz.Logging;
using Quartz;
using CryptoExchangeBot.Quartz.Jobs;
using CryptoExchangeBot.Models.Settings;
using System.Threading;

namespace CryptoExchangeBot.Quartz
{
    public static class DataScheduler
    {
        private static IScheduler _scheduler;
        private static QuartzSettings _settings;

        /// <summary>
        /// Настраивает джобы для работы с уведомлениями и парсингом.
        /// </summary>
        public static async Task Initialize(IServiceProvider serviceProvider, QuartzSettings quartzSettings, CancellationToken cancellationToken)
        {
            _settings = quartzSettings;

            LogProvider.SetCurrentLogProvider(new CustomLogProvider());

            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = serviceProvider.GetService<JobFactory>();

            await ScheduleDailyMorningMessageJob(cancellationToken);
            await ScheduleDailyEveningMessageJob(cancellationToken);
            await ScheduleFinishedMessageJob(cancellationToken);
            await ScheduleUpdateUserResultsJob(cancellationToken);

            await _scheduler.Start(cancellationToken);
        }

        private static async Task ScheduleDailyMorningMessageJob(CancellationToken cancellationToken)
        {
            var job = JobBuilder.Create<SendDailyMorningMessageJob>()
                .WithIdentity("SendDailyMorningMessageJob", "DailyMorningMessage")
                .Build();

            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity("SendDailyMorningMessageTrigger", "DailyMorningMessage")
                .StartNow();

            if (_settings.DateStart > DateTime.Now)
            {
                triggerBuilder.StartAt(_settings.DateStart);
            }
            else
            {
                triggerBuilder.StartNow();
            }

            var trigger = triggerBuilder.EndAt(_settings.DateEnd)
                .WithCronSchedule(_settings.ExecutingMorningTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        private static async Task ScheduleDailyEveningMessageJob(CancellationToken cancellationToken)
        {
            var job = JobBuilder.Create<SendDailyEveningMessageJob>()
                .WithIdentity("SendDailyEveningMessageJob", "DailyEveningMessage")
                .Build();

            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity("SendDailyEveningMessageTrigger", "DailyEveningMessage")
                .StartNow();

            if (_settings.DateStart > DateTime.Now)
            {
                triggerBuilder.StartAt(_settings.DateStart);
            }
            else
            {
                triggerBuilder.StartNow();
            }

            var trigger = triggerBuilder.EndAt(_settings.DateEnd)
                .WithCronSchedule(_settings.ExecutingEveningTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        private static async Task ScheduleFinishedMessageJob(CancellationToken cancellationToken)
        {
            if (_settings.FinishedMessageDate < DateTime.Now)
            {
                return;
            }

            var job = JobBuilder.Create<SendFinishedMessageJob>()
                .WithIdentity("SendFinishedMessageJob", "FinishedMessage")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("SendFinishedMessageTrigger", "FinishedMessage")
                .StartAt(_settings.FinishedMessageDate)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        private static async Task ScheduleUpdateUserResultsJob(CancellationToken cancellationToken)
        {
            var job = JobBuilder.Create<UpdateUserResultsJob>()
                .WithIdentity("UpdateUserResultsJob", "UpdateUserResults")
                .Build();

            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity("UpdateUserResultsTrigger", "UpdateUserResults")
                .StartNow();

            if (_settings.DateStart > DateTime.Now)
            {
                triggerBuilder.StartAt(_settings.DateStart);
            }
            else
            {
                triggerBuilder.StartNow();
            }

            var trigger = triggerBuilder.EndAt(_settings.DateEnd)
                .WithCronSchedule(_settings.ExecutingUpdateUserResultsTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        /// <summary>
        /// Получаем предыдущее время выполнения тригера
        /// </summary>
        /// <returns>Предыдущее время выполнения тригера</returns>
        public static async Task<DateTime> GetDailyEveningTriggerPreviousFireTime()
        {
            var dailyEveningTrigger = await _scheduler.GetTrigger(new TriggerKey("DailyEveningMessage", "SendDailyEveningMessageTrigger"));
            var previousFireTimeUtc = dailyEveningTrigger.GetPreviousFireTimeUtc().Value.ToLocalTime().DateTime;

            return previousFireTimeUtc;
        }
    }
}
