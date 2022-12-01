using System.ComponentModel.DataAnnotations;

namespace CryptoExchangeBot.Models.Entities
{
    public class User
    {
        [Key]
        public long ChatId { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Phone { get; set; }
    }
}
