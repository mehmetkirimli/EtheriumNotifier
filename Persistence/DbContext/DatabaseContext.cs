using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DbContext
{
    public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<NotificationChannel> NotificationChannels { get; set; } = null!;
        public DbSet<ExternalTransaction> ExternalTransactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Transaction - Hash kolonu unıque index
            // Aynı işlemi 2.kez kaydetmemek için güvenlik önlemi bu


            modelBuilder.Entity<ExternalTransaction>()
           .HasIndex(e => e.TransactionHash)
           .IsUnique(); // idempotency için önemli

            // Enum → string gösterim (elzem değil ama okunabilirlik için iyi)
            modelBuilder.Entity<NotificationChannel>()
                .Property(x => x.ChannelType)
                .HasConversion<string>();

            modelBuilder.Entity<Notification>()
                .Property(x => x.ChannelType)
                .HasConversion<string>();


            base.OnModelCreating(modelBuilder);
        }
       }
}
