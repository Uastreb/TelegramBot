﻿using System;

namespace TelegramBot.Forms.Models
{
    [Serializable]
    public class GroupQuestions
    {
        public string GroupName { get; set; }
        public string Introduction { get; set; }
        public Question[] Questions { get; set; }
    }
}
