using System;

namespace TelegramBot.Forms.Models
{
    [Serializable]
    public class Question
    {
        public string QuestionText { get; set; }
        public Answer[] AnswerOptions { get; set; }
    }
}
