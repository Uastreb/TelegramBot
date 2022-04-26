using System;

namespace TelegramBot.Forms.Models
{
    [Serializable]
    public class Answer
    {
        public string AnswerOption { get; set; }
        public double AnswerPoints { get; set; }
    }
}
