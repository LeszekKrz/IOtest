using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Respawn;
using Respawn.Graph;
using YouTubeV2.Application;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Api.Tests
{
    internal class Setup
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
                    "AspNetRoles",
                }
            });

            await respawner.ResetAsync(connection);
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"appsettings.test.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureServices(services =>
                {
                    var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(YTContext));
                    if (context != null)
                    {
                        services.Remove(context);
                        var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                          || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                        foreach (var option in options)
                        {
                            services.Remove(option);
                        }
                    }

                    services.AddDbContext<YTContext>(
                        options => options.UseSqlServer(config.GetConnectionString("Db")));
                });
            });
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactory<T>(T service) where T : class
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"..\..\..\appsettings.test.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureServices(services =>
                {
                    var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(YTContext));
                    if (context != null)
                    {
                        services.Remove(context);
                        var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                          || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                        foreach (var option in options)
                        {
                            services.Remove(option);
                        }
                    }

                    services.AddDbContext<YTContext>(
                        options => options.UseSqlServer(config.GetConnectionString("Db")));

                    services.Replace(new ServiceDescriptor(typeof(T), service));
                });
            });
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactory<T1, T2>(T1 service1, T2 service2)
            where T1 : class
            where T2 : class
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"..\..\..\appsettings.test.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureServices(services =>
                {
                    var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(YTContext));
                    if (context != null)
                    {
                        services.Remove(context);
                        var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                          || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                        foreach (var option in options)
                        {
                            services.Remove(option);
                        }
                    }

                    services.AddDbContext<YTContext>(
                        options => options.UseSqlServer(config.GetConnectionString("Db")));

                    services.Replace(new ServiceDescriptor(typeof(T1), service1));
                    services.Replace(new ServiceDescriptor(typeof(T2), service2));
                });
            });
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactory<T1, T2, T3>(T1 service1, T2 service2, T3 service3)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"..\..\..\appsettings.test.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureServices(services =>
                {
                    var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(YTContext));
                    if (context != null)
                    {
                        services.Remove(context);
                        var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                          || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                        foreach (var option in options)
                        {
                            services.Remove(option);
                        }
                    }

                    services.AddDbContext<YTContext>(
                        options => options.UseSqlServer(config.GetConnectionString("Db")));

                    services.Replace(new ServiceDescriptor(typeof(T1), service1));
                    services.Replace(new ServiceDescriptor(typeof(T2), service2));
                    services.Replace(new ServiceDescriptor(typeof(T3), service3));
                });
            });
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactoryWithVideoProcessingServiceMocked(IVideoProcessingService videoProcessingService)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"..\..\..\appsettings.test.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureServices(services =>
                {
                    var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(YTContext));
                    if (context != null)
                    {
                        services.Remove(context);
                        var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                          || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                        foreach (var option in options)
                        {
                            services.Remove(option);
                        }
                    }

                    services.AddDbContext<YTContext>(
                        options => options.UseSqlServer(config.GetConnectionString("Db")));

                    services.Replace(new ServiceDescriptor(typeof(IVideoProcessingService), videoProcessingService));

                    var hostedServiceDescriptor = services.SingleOrDefault(d => d.ImplementationFactory?.Method.ReturnType == typeof(VideoProcessingService));
                    if (hostedServiceDescriptor != null)
                        services.Remove(hostedServiceDescriptor);
                });
            });
        }
    }
}
