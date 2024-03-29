﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Entity;

namespace TelegramBot
{
    class Program
    {
        private static ITelegramBotClient _telegramBotClient;

        private static List<UserPoll> _userPolls = new List<UserPoll>();

        private static string token = string.Empty;
        private static string pathToDocWithResults = string.Empty;
        private static string pathToXmlWithUserPolls = string.Empty;
        private static string pathToXmlWithPoll = string.Empty;

        private static GoogleSheetService _googleSheetService;

        private static CancellationTokenSource _updateDataCTS;

        private static Settings settings;

        private static Poll _poll;

        static void Main(string[] args)
        {
            try
            {
                settings = XmlSerializationHelper<Settings>.DeserializeFromFile(@"Settings.xml");

                if (!string.IsNullOrEmpty(settings.GoogleSheetsAppName) &&
                    !string.IsNullOrEmpty(settings.GoogleSpreadsheetId) &&
                    File.Exists("telegrambot-settings.json"))
                {
                    _googleSheetService = new GoogleSheetService(settings.GoogleSheetsAppName, settings.GoogleSpreadsheetId);
                }

                token = settings.Token;
                pathToDocWithResults = settings.PathToDocWithResults;
                pathToXmlWithUserPolls = settings.PathToXmlWithUserPolls;
                pathToXmlWithPoll = settings.PathToXmlWithPoll;

                _poll = XmlSerializationHelper<Poll>.DeserializeFromFile(pathToXmlWithPoll);

                LoadUserPolls();

                _updateDataCTS = new CancellationTokenSource();
                Task.Run(() => UpdateData());

                _telegramBotClient = new TelegramBotClient(token);
                _telegramBotClient.StartReceiving();
                _telegramBotClient.OnMessage += _telegramBotClient_OnMessage;

                Console.WriteLine("Введите Cancel для прекращения работы и выхода");
                var text = string.Empty;
                do
                {
                    text = Console.ReadLine();
                }
                while (!string.Equals(text, "Cancel", StringComparison.OrdinalIgnoreCase));

                _telegramBotClient.StopReceiving();
                _updateDataCTS.Cancel();

                SaveUserPolls();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Проиошла ошибка{Environment.NewLine}Проверьте правильно ли настроен файл настроек{Environment.NewLine}{Environment.NewLine}{ex}");
                Console.ReadLine();
            }
        }

        private static async Task UpdateData()
        {
            try
            {
                for (; ; )
                {
                    SaveUserPolls();

                    LoadUserPolls();

                    await Task.Delay(TimeSpan.FromHours(1));
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Бот остановлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при отправке письма\n{ex}");
            }
        }

        private static async void _telegramBotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id;
            UserPoll userPoll = default;

            if (!_userPolls.Any(x => x.UserId == userId))
            {
                userPoll = new UserPoll()
                {
                    Poll = XmlSerializationHelper<Poll>.CreateDeepCopy(_poll),
                    UserId = message.From.Id
                };

                _userPolls.Add(userPoll);
                await SendTextMessageAsync(message, _poll.Introduction);
            }

            userPoll = _userPolls.First(x => x.UserId == userId);
            var poll = userPoll.Poll;

            if (await HandleCurrentUserData(poll, message))
            {
                return;
            }

            var currentQuestionGroup = await HandleCurrentQuestionGroup(message, poll);
            if (currentQuestionGroup == default)
            {
                return;
            }

            var currentQuestion = await HandleCurrentQuestion(currentQuestionGroup, message);
            if (currentQuestion == default)
            {
                return;
            }

            if (await HandleNextQuestion(currentQuestionGroup, message))
            {
                return;
            }

            if (await HandleResult(poll, userPoll, message))
            {
                return;
            }

            await HandleNextQuestionGroup(poll, message, sender, e);

            return;
        }

        /// <summary>
        /// Отправляет сообщение о вводе текущих данных пользователя, если сообщение было отправлено записывает результат,
        /// отправляет следующее сообщение
        /// </summary>
        /// <param name="poll">Опрос</param>
        /// <param name="message">Полученное сообщение</param>
        /// <returns>Нужно ли прекращать выполнение после</returns>
        private static async Task<bool> HandleCurrentUserData(Poll poll, Telegram.Bot.Types.Message message)
        {
            var currentUserData = poll.UserDatas.FirstOrDefault(x => !x.ValueIsEntered);
            if (currentUserData != default && !currentUserData.TextIsSent)
            {
                await SendTextMessageAsync(message, currentUserData.Text);
                currentUserData.TextIsSent = true;

                return true;
            }
            else if (currentUserData != default && currentUserData.TextIsSent)
            {
                currentUserData.Value = message.Text;
                currentUserData.ValueIsEntered = true;

                var returnAfterExecute = await HandleNextUserData(poll, message);

                return returnAfterExecute;
            }

            return false;
        }

        /// <summary>
        /// Отправляет сообщение о вводе следующих данных о пользователе
        /// </summary>
        /// <param name="poll">Опрос</param>
        /// <param name="message">Полученное сообщение</param>
        /// <returns>Нужно ли прекращать выполнение после</returns>
        private static async Task<bool> HandleNextUserData(Poll poll, Telegram.Bot.Types.Message message)
        {
            var nextUserData = poll.UserDatas.FirstOrDefault(x => !x.ValueIsEntered);
            if (nextUserData != default)
            {
                await SendTextMessageAsync(message, nextUserData.Text);
                nextUserData.TextIsSent = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Обрабатывает следующую группу вопросов
        /// </summary>
        /// <param name="poll">Опрос</param>
        /// <param name="message">Полученное сообщение</param>
        private static async Task HandleNextQuestionGroup(Poll poll, Telegram.Bot.Types.Message message,
            object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var nextQuestionGroup = poll.QuestionGroups.First(x => !x.IsPassed);

            await SendTextMessageAsync(message, nextQuestionGroup.Introduction);
            nextQuestionGroup.TextIsSent = true;

            _telegramBotClient_OnMessage(sender, e);
        }

        /// <summary>
        /// Если пользователь ответил на все группы - Выводит что он уже прошел тест,
        /// иначе возвращает первую неотвеченную группу вопросов
        /// </summary>
        /// <param name="message">
        /// Полученное сообщение
        /// </param>
        /// <param name="poll">
        /// Опрос пользователя
        /// </param>
        /// <returns>
        /// Текущую группу вопросов
        /// </returns>
        private static async Task<GroupQuestions> HandleCurrentQuestionGroup(Telegram.Bot.Types.Message message, Poll poll)
        {
            var currentQuestionGroup = poll.QuestionGroups.FirstOrDefault(x => !x.IsPassed);
            if (currentQuestionGroup == default)
            {
                await SendTextMessageAsync(message, $"Вы уже прошли этот тест.");

                return default;
            }

            if (!currentQuestionGroup.TextIsSent && !string.IsNullOrEmpty(currentQuestionGroup.Introduction))
            {
                await SendTextMessageAsync(message, currentQuestionGroup.Introduction);
                currentQuestionGroup.TextIsSent = true;
            }

            return currentQuestionGroup;
        }

        /// <summary>
        /// Если больше нет вопросов, записывает результат, отправляет пользователю заключение
        /// </summary>
        /// <param name="poll">Опрос</param>
        /// <param name="userPoll">Опрос пользователя</param>
        /// <param name="message">Полученное сообщение</param>
        /// <returns>Надо ли прекращать выполнение метода</returns>
        private static async Task<bool> HandleResult(Poll poll, UserPoll userPoll, Telegram.Bot.Types.Message message)
        {
            if (poll.QuestionGroups.All(x => x.IsPassed))
            {
                await WriteResult(userPoll, poll, message);
                await SendTextMessageAsync(message, poll.Conclusion,
                    replyMarkup: new ReplyKeyboardRemove());

                return true;
            }

            return false;
        }

        /// <summary>
        /// Отправляет следующий вопрос пользователю, если есть неотвеченные вопросы
        /// </summary>
        /// <param name="currentQuestionGroup">
        /// Текущая группа вопросов
        /// </param>
        /// <param name="message">
        /// Полученное сообщение
        /// </param>
        /// <returns>
        /// Надо ли прекращать выполнение метода
        /// </returns>
        private static async Task<bool> HandleNextQuestion(GroupQuestions currentQuestionGroup, Telegram.Bot.Types.Message message)
        {
            if (currentQuestionGroup.Questions.Any(x => !x.IsPassed))
            {
                var nextQuestion = currentQuestionGroup.Questions.FirstOrDefault(x => !x.IsPassed);
                var answerButtons = GetButtons(nextQuestion);

                await SendTextMessageAsync(message, nextQuestion.QuestionText, replyMarkup: answerButtons);
                nextQuestion.TextIsSent = true;

                return true;
            }

            currentQuestionGroup.IsPassed = true;

            return false;
        }

        /// <summary>
        /// Обрабатывает текущий вопрос
        /// </summary>
        /// <param name="currentQuestionGroup">
        /// Текущая группа вопросов
        /// </param>
        /// <param name="message">
        /// Полученное сообщение
        /// </param>
        /// <returns>
        /// Текущий вопрос
        /// </returns>
        private static async Task<Question> HandleCurrentQuestion(GroupQuestions currentQuestionGroup, Telegram.Bot.Types.Message message)
        {
            var currentQuestion = currentQuestionGroup.Questions.FirstOrDefault(x => !x.IsPassed);
            var currentAnswer = currentQuestion.AnswerOptions.FirstOrDefault(x => string.Equals(x.AnswerOption, message.Text,
                StringComparison.OrdinalIgnoreCase));
            if (currentAnswer == default)
            {
                var answerButtons = GetButtons(currentQuestion);

                await SendTextMessageAsync(message, currentQuestion.QuestionText, replyMarkup: answerButtons);
                currentQuestion.TextIsSent = true;

                return default;
            }

            currentQuestion.PointsReceived = currentAnswer.AnswerPoints;
            currentQuestion.IsPassed = true;

            return currentQuestion;
        }

        private static async Task SendTextMessageAsync(Telegram.Bot.Types.Message message, string text, IReplyMarkup replyMarkup = default)
        {
            try
            {
                if (!string.IsNullOrEmpty(text) && replyMarkup != default)
                {
                    await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: replyMarkup);
                }
                else if (!string.IsNullOrEmpty(text) && replyMarkup == default)
                {
                    await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при отправке письма\nЧат:{message.Chat.Id}\nТип:{message.Chat.Type}\nПисьмо от, логин:{message.From.Username}\n" +
                    $"Письмо от, иф:{message.From.FirstName} {message.From.LastName}\n{ex}");
            }
        }

        private static async Task SendTextMessageAsync(long chatId, string text, IReplyMarkup replyMarkup = default)
        {
            try
            {
                if (!string.IsNullOrEmpty(text) && replyMarkup != default)
                {
                    await _telegramBotClient.SendTextMessageAsync(chatId, text, replyMarkup: replyMarkup);
                }
                else if (!string.IsNullOrEmpty(text) && replyMarkup == default)
                {
                    await _telegramBotClient.SendTextMessageAsync(chatId, text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при отправке письма администратору \nЧат:{chatId}\n{ex}");
            }
        }

        private static IReplyMarkup GetButtons(Question question)
        {
            var answers = question.AnswerOptions.Select(x => x.AnswerOption).Split(3);
            var keyboardButtons = answers.Select(x => x.Select(i => new KeyboardButton(i)));

            var replyKeyboardMarkup = new ReplyKeyboardMarkup
            {
                Keyboard = keyboardButtons
            };

            return replyKeyboardMarkup;
        }

        /// <summary>
        /// Записывает результат опроса в файл
        /// </summary>
        /// <param name="userPoll">Опрос пользователя</param>
        /// <param name="poll">Опрос</param>
        /// <param name="user">Пользователь</param>
        private static async Task WriteResult(UserPoll userPoll, Poll poll, Telegram.Bot.Types.Message message)
        {
            double totalResult = 0;
            StringBuilder result = new StringBuilder();

            var userInitials = $"{DateTime.Now}{Environment.NewLine}{message.From.FirstName} {message.From.LastName}";

            result.AppendLine(userInitials);

            if (settings.WriteChatId)
            {
                result.AppendLine($"Chat id: {message.Chat.Id}");
            }

            foreach (var userData in poll.UserDatas)
            {
                result.AppendLine($"{userData.Name}: {userData.Value}");
            }

            foreach (var questionGroup in userPoll.Poll.QuestionGroups)
            {
                var groupAverage = questionGroup.Questions.Average(x => x.PointsReceived);
                result.AppendLine($"Группа: {questionGroup.GroupName} - {groupAverage} балла(ов)");
                totalResult += groupAverage;
            }

            totalResult /= userPoll.Poll.QuestionGroups.Count();
            result.AppendLine($"Среднее по всем группам: {totalResult} балла(ов)");
            result.AppendLine();

            var resultText = result.ToString();

            File.AppendAllText(pathToDocWithResults, resultText);

            if (settings.AdminChatId.HasValue)
            {
                await SendTextMessageAsync(settings.AdminChatId.Value, resultText);
            }

            if (_googleSheetService != null && userPoll != default)
            {
                var googleSheetsResult = GetResultForGoogleSheet(userPoll);

                _googleSheetService.WriteData(googleSheetsResult, _userPolls.Count + 2);
            }
        }

        private static void SaveUserPolls()
        {
            var userPolls = _userPolls.ToArray();

            if (File.Exists(pathToXmlWithUserPolls))
            {
                File.Delete(pathToXmlWithUserPolls);
            }

            XmlSerializationHelper<UserPoll[]>.SerializeInFile(userPolls, pathToXmlWithUserPolls);
        }

        private static void LoadUserPolls()
        {
            var userPolls = XmlSerializationHelper<UserPoll[]>.DeserializeFromFile(pathToXmlWithUserPolls);

            lock (_userPolls)
            {
                _userPolls = userPolls.ToList();

                if (_userPolls == default)
                {
                    _userPolls = new List<UserPoll>();
                }
            }

            if (_googleSheetService != null && _userPolls != default && _userPolls.Any())
            {
                var googleSheetsResults = GetResultsForGoogleSheet(_userPolls);
                var googleSheetsTitles = GetTitlesForGoogleSheet(_userPolls);

                _googleSheetService.WriteData(googleSheetsTitles, 1);
                _googleSheetService.WriteData(googleSheetsResults, 2);
            }
        }

        private static IList<IList<object>> GetResultsForGoogleSheet(List<UserPoll> userPolls)
        {
            var googleSheetRows = new List<IList<object>>();

            foreach (var userPoll in userPolls)
            {
                double totalResult = 0;
                var googleSheetColumns = new List<object>();

                googleSheetColumns.Add(userPoll.UserId);

                foreach (var userData in userPoll.Poll.UserDatas)
                {
                    googleSheetColumns.Add(userData.Value);
                }

                foreach (var questionGroup in userPoll.Poll.QuestionGroups)
                {
                    var groupAverage = questionGroup.Questions.Average(x => x.PointsReceived);
                    googleSheetColumns.Add($"{groupAverage} балла(ов)");
                    totalResult += groupAverage;
                }

                totalResult /= userPoll.Poll.QuestionGroups.Count();
                googleSheetColumns.Add($"{totalResult} балла(ов)");

                googleSheetRows.Add(googleSheetColumns);
            }

            return googleSheetRows;
        }

        private static IList<IList<object>> GetResultForGoogleSheet(UserPoll userPoll)
        {
            var googleSheetRows = new List<IList<object>>();

            double totalResult = 0;
            var googleSheetColumns = new List<object>();

            foreach (var userData in userPoll.Poll.UserDatas)
            {
                googleSheetColumns.Add(userData.Value);
            }

            foreach (var questionGroup in userPoll.Poll.QuestionGroups)
            {
                var groupAverage = questionGroup.Questions.Average(x => x.PointsReceived);
                googleSheetColumns.Add($"{groupAverage} балла(ов)");
                totalResult += groupAverage;
            }

            totalResult /= userPoll.Poll.QuestionGroups.Count();
            googleSheetColumns.Add($"{totalResult} балла(ов)");

            googleSheetRows.Add(googleSheetColumns);

            return googleSheetRows;
        }

        private static IList<IList<object>> GetTitlesForGoogleSheet(List<UserPoll> userPolls)
        {
            var googleSheetRows = new List<IList<object>>();

            var firstUserPoll = userPolls.First();
            var googleSheetColumns = new List<object>();

            googleSheetColumns.Add("Id пользователя");

            foreach (var userData in firstUserPoll.Poll.UserDatas)
            {
                googleSheetColumns.Add(userData.Name);
            }

            foreach (var questionGroup in firstUserPoll.Poll.QuestionGroups)
            {
                googleSheetColumns.Add($"Кол-во баллов по группе {questionGroup.GroupName}");
            }

            googleSheetColumns.Add($"Среднее по всем группам");

            googleSheetRows.Add(googleSheetColumns);

            return googleSheetRows;
        }
    }
}
