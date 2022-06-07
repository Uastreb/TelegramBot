using System;
using System.Collections.Generic;
using Data = Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json;
using System.IO;

namespace TelegramBot
{
    public class GoogleSheetService
    {
        private string[] _scopes = { SheetsService.Scope.Spreadsheets }; // Change this if you're accessing Drive or Docs
        private readonly string _applicationName = "TelegramBot";
        private readonly string _spreadsheetId = "1mUuhymXZzSA5OkVQko8K529R762z7hC6GBrImfxWHk0";
        private SheetsService _sheetsService;

        public GoogleSheetService(string applicationName, string spreadsheetId)
        {
            _applicationName = applicationName;
            _spreadsheetId = spreadsheetId;
            GoogleCredential credential;

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy 
            using (var stream = new FileStream("telegrambot-settings.json",
                FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }

            // Create Google Sheets API service.
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }


        public string WriteData(IList<IList<object>> data, int row)
        {
            String range = $"Answers!A{row}:Y";
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<Data.ValueRange> updateData = new List<Data.ValueRange>();
            var dataValueRange = new Data.ValueRange();
            dataValueRange.Range = range;
            dataValueRange.Values = data;
            updateData.Add(dataValueRange);

            Data.BatchUpdateValuesRequest requestBody = new Data.BatchUpdateValuesRequest();
            requestBody.ValueInputOption = valueInputOption;
            requestBody.Data = updateData;

            var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

            Data.BatchUpdateValuesResponse response = request.Execute();
            // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

            return JsonConvert.SerializeObject(response);
        }
    }
}
