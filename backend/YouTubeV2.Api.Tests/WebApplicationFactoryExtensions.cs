using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using YouTubeV2.Api.Tests.Handlers;
using YouTubeV2.Api.Tests.Providers;

namespace YouTubeV2.Api.Tests
{
    internal static class WebApplicationFactoryExtensions
    {
        internal static async Task DoWithinScope<TService>(
            this WebApplicationFactory<Program> webApplicationFactory, Func<TService, Task> action)
        where TService : notnull
        {
            using var serviceScope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var service = serviceScope.ServiceProvider.GetRequiredService<TService>();
            await action(service);
        }

        internal static async Task DoWithinScope<TService, UService>(
            this WebApplicationFactory<Program> webApplicationFactory, Func<TService, UService, Task> action)
            where TService : notnull
            where UService : notnull
        {
            using var serviceScope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var tService = serviceScope.ServiceProvider.GetRequiredService<TService>();
            var uService = serviceScope.ServiceProvider.GetRequiredService<UService>();
            await action(tService, uService);
        }

        internal static async Task<TResult> DoWithinScopeWithReturn<TService, TResult>(
            this WebApplicationFactory<Program> webApplicationFactory, Func<TService, Task<TResult>> action)
            where TService : notnull
        {
            using var serviceScope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var tService = serviceScope.ServiceProvider.GetRequiredService<TService>();
            return await action(tService);
        }

        internal static WebApplicationFactory<T> WithAuthentication<T>(
            this WebApplicationFactory<T> webApplicationFactory,
            ClaimsProvider claimsProvider)
            where T : class =>
            webApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.schemeName;
                        options.DefaultChallengeScheme = TestAuthenticationHandler.schemeName;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.schemeName, options => { });
                    services.AddScoped(_ => claimsProvider);
                });
            });
    }
}
