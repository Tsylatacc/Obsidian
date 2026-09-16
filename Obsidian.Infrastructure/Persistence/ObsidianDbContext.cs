using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;

namespace Obsidian.Infrastructure.Persistence
{
    public class ObsidianDbContext : DbContext
    {
        public ObsidianDbContext(DbContextOptions<ObsidianDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ObsidianDbContext).Assembly
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Channel> Channels { get; set; }
    }
}
