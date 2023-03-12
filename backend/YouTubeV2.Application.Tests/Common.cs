using Respawn.Graph;
using Respawn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace YouTubeV2.Application.Tests
{
    internal static class Common
    {
        public static YTContext SetUpContext(string connection)
        {
            var dbOption = new DbContextOptionsBuilder<YTContext>()
                .UseSqlServer(connection)
                .Options;
            return new YTContext(dbOption);
        }

        public static async Task ResetDatabaseAsync(string connection)
        {
            var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = new Table[]
                    {
                        "__EFMigrationsHistory",
                        "JobTypes",
                        "NationalIdTypes"
                    }
            });

            await respawner.ResetAsync(connection);
        }

        public static IConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            return config;
        }
    }
}
