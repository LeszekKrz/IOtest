using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.EntityConfiguration;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application
{
    public class YTContext : DbContext
    {
        public DbSet<User> Users { get; private set; }

        public YTContext(DbContextOptions<YTContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
        }
    }
}
