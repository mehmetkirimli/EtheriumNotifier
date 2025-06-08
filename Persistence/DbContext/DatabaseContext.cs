using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DbContext
{
    public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DatabaseContext(Microsoft.EntityFrameworkCore.DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<NotificationChannel> NotificationChannels { get; set; } = null!;
        public DbSet<Domain.Entities.Transaction> Transactions { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Transaction - Hash kolonu unıque index
            // Aynı işlemi 2.kez kaydetmemek için güvenlik önlemi bu
            modelBuilder.Entity<Transaction>()
           .HasIndex(x => x.Hash)
           .IsUnique();


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
