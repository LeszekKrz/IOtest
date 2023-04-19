using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.EntityConfiguration;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application
{
    public class YTContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public YTContext(DbContextOptions<YTContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);

            var roles = new[]
            {
                new Role
                {
                    Id = "39cc2fe2-d00d-4f48-a49d-005d8e983c72",
                    Name = Role.Simple,
                    NormalizedName = Role.Simple.ToUpper(),
                },
                new Role
                {
                    Id = "63798117-72aa-4bc5-a1ef-4e771204d561",
                    Name = Role.Creator,
                    NormalizedName = Role.Creator.ToUpper(),
                },
                new Role
                {
                    Id = "b3a48a48-1a74-45da-a179-03b298bc53bc",
                    Name = Role.Administrator,
                    NormalizedName = Role.Administrator.ToUpper(),
                }
            };

            modelBuilder.Entity<Role>().HasData(roles);          

            var simple = new User
            {
                Id = "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                Email = "simple@test.com",
                NormalizedEmail = "simple@test.com".ToUpper(),
                Name = "Simple",
                Surname = "Test",
                UserName = "TestSimple",
                NormalizedUserName = "TestSimple".ToUpper(),
            };

            var creator = new User
            {
                Id = "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                Email = "creator@test.com",
                NormalizedEmail = "creator@test.com".ToUpper(),
                Name = "Creator",
                Surname = "Test",
                UserName = "TestCreator",
                NormalizedUserName = "TestCreator".ToUpper(),
            };

            var admin = new User
            {
                Id = "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                Email = "admin@test.com",
                NormalizedEmail = "admin@test.com".ToUpper(),
                Name = "Admin",
                Surname = "Test",
                UserName = "TestAdmin",
                NormalizedUserName = "TestAdmin".ToUpper(),
            };

            PasswordHasher<User> ph = new();
            simple.PasswordHash = ph.HashPassword(simple, "123!@#asdASD");
            creator.PasswordHash = ph.HashPassword(creator, "123!@#asdASD");
            admin.PasswordHash = ph.HashPassword(admin, "123!@#asdASD");

            modelBuilder.Entity<User>().HasData(simple);
            modelBuilder.Entity<User>().HasData(creator);
            modelBuilder.Entity<User>().HasData(admin);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = roles[0].Id,
                    UserId = simple.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = roles[1].Id,
                    UserId = creator.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = roles[2].Id,
                    UserId = admin.Id
                });
        }
    }
}
