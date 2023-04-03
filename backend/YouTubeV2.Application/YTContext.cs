using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.EntityConfiguration;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application
{
    public class YTContext : IdentityDbContext<
        User,
        Role,
        string>
    {
        public DbSet<Subscription> Subscriptions { get; set; }

        public YTContext(DbContextOptions<YTContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
        }
    }
}
