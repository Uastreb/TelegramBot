using CryptoExchangeBot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchangeBot.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DailyEarning> DailyEarnings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DailyEarning>().HasKey(nameof(DailyEarning.ChatId), nameof(DailyEarning.DateCreated));
        }

    }
}
