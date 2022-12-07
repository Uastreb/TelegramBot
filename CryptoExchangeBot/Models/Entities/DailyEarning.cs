using System.ComponentModel.DataAnnotations;

namespace CryptoExchangeBot.Models.Entities
{
    public class DailyEarning
    {
        public long ChatId { get; set; }
        public double? Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public string Comment { get; set; }
    }
}
