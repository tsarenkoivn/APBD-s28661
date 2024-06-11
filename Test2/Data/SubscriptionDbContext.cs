using Microsoft.EntityFrameworkCore;
using Test2.Models;

namespace Test2.Data
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                        .HasKey(c => c.IdClient);

            modelBuilder.Entity<Discount>()
                        .HasKey(d => d.IdDiscount);
            modelBuilder.Entity<Discount>()
                        .HasOne(d => d.Client)
                        .WithMany(c => c.Discounts)
                        .HasForeignKey(d => d.IdClient);

            modelBuilder.Entity<Payment>()
                        .HasKey(p => p.IdPayment);
            modelBuilder.Entity<Payment>()
                        .HasOne(p => p.Client)
                        .WithMany(c => c.Payments)
                        .HasForeignKey(p => p.IdClient);
            modelBuilder.Entity<Payment>()
                        .HasOne(p => p.Subscription)
                        .WithMany(s => s.Payments)
                        .HasForeignKey(p => p.IdSubscription);

            modelBuilder.Entity<Sale>()
                        .HasKey(s => s.IdSale);
            modelBuilder.Entity<Sale>()
                        .HasOne(s => s.Client)
                        .WithMany(c => c.Sales)
                        .HasForeignKey(s => s.IdClient);
            modelBuilder.Entity<Sale>()
                        .HasOne(s => s.Subscription)
                        .WithMany(s => s.Sales)
                        .HasForeignKey(s => s.IdSubscription);

            modelBuilder.Entity<Subscription>()
                        .HasKey(s => s.IdSubscription);
        }
    }
}
