using CryptoExchangeBot.Data;
using Microsoft.Extensions.Options;
using Quartz;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using CryptoExchangeBot.Models.Settings.GoogleSheets;
using Serilog;
using ILogger = Serilog.ILogger;

namespace CryptoExchangeBot.Quartz.Jobs
{
    public class UpdateUserResultsJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;

        private readonly GoogleSheetsConfiguration _googleSheetsConfiguration;

        public UpdateUserResultsJob(IServiceScopeFactory serviceScopeFactory, 
            IOptions<GoogleSheetsConfiguration> googleSheetsConfigurationOpt, 
            ILogger logger)
        {
            _googleSheetsConfiguration = googleSheetsConfigurationOpt.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    IList<IList<object>> speedSheets = new List<IList<object>>();

                    var dbContext = scope.ServiceProvider.GetService<ApplicationContext>();
                    var sheetsService = scope.ServiceProvider.GetService<SheetsService>();

                    foreach (var user in dbContext.Users)
                    {
                        var userDailyEarnings = dbContext.DailyEarnings.Where(x => x.ChatId == user.ChatId);

                        var dateRow = new List<object>();
                        dateRow.Add(user.Name);
                        dateRow.AddRange(userDailyEarnings.Select(x => x.DateCreated.ToShortDateString()));

                        var amountRow = new List<object>();
                        amountRow.Add("Кол-во баллов");
                        amountRow.AddRange(userDailyEarnings.Select(x => x.Amount.HasValue ? x.Amount.ToString() : String.Empty));

                        var commentRow = new List<object>();
                        commentRow.Add(userDailyEarnings.Count(x => x.Amount.HasValue));
                        commentRow.AddRange(userDailyEarnings.Select(x => x.Comment));

                        speedSheets.Add(dateRow);
                        speedSheets.Add(amountRow);
                        speedSheets.Add(commentRow);
                    }

                    await WriteData(speedSheets, sheetsService);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                }
            }
        }

        private async Task WriteData(IList<IList<object>> data, SheetsService sheetsService)
        {
            String range = $"Answers!A1:BS";
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();
            var dataValueRange = new ValueRange();
            dataValueRange.Range = range;
            dataValueRange.Values = data;
            updateData.Add(dataValueRange);

            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
            requestBody.ValueInputOption = valueInputOption;
            requestBody.Data = updateData;

            var request = sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _googleSheetsConfiguration.SpeadsheetId);

            BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 
        }
    }
}
