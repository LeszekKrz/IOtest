using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.DonationControllerTests
{
    [TestClass]
    public class WithdrawMoneyAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task WithdrawMoney_GoodAmmount_ShouldRemoveFromBallance()
        {
            // ARRANGE
            User withdrawer = new User("withdrawer1@mail.com", "withdrawer1", "withdrawer1", "withdrawer1");
            withdrawer.AccountBalance = 20;
            string withdrawerId = null!;


            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(withdrawer);
                    await userManager.AddToRoleAsync(withdrawer, Role.Creator);

                    withdrawerId = await userManager.GetUserIdAsync(withdrawer);
                });

            using HttpClient httpClient = _webApplicationFactory
                .WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, withdrawerId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "amount", "10" }
            };
            var path = QueryHelpers.AddQueryString("api/donate/withdraw", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    var withdrawerFound = await userManager.FindByIdAsync(withdrawerId);

                    withdrawerFound!.AccountBalance.Should().Be(10);
                });
        }

        [TestMethod]
        public async Task WithdrawMoney_NegativeAmmount_ShouldReturnBadRequest()
        {
            // ARRANGE
            User withdrawer = new User("withdrawer2@mail.com", "withdrawer2", "withdrawer2", "withdrawer2");
            withdrawer.AccountBalance = 20;
            string withdrawerId = null!;


            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(withdrawer);
                    await userManager.AddToRoleAsync(withdrawer, Role.Creator);

                    withdrawerId = await userManager.GetUserIdAsync(withdrawer);
                });

            using HttpClient httpClient = _webApplicationFactory
                .WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, withdrawerId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "amount", "-10" }
            };
            var path = QueryHelpers.AddQueryString("api/donate/withdraw", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task WithdrawMoney_NotEnoughAmmountAmmount_ShouldReturnBadRequest()
        {
            // ARRANGE
            User withdrawer = new User("withdrawer1@mail.com", "withdrawer1", "withdrawer1", "withdrawer1");
            withdrawer.AccountBalance = 20;
            string withdrawerId = null!;


            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(withdrawer);
                    await userManager.AddToRoleAsync(withdrawer, Role.Creator);

                    withdrawerId = await userManager.GetUserIdAsync(withdrawer);
                });

            using HttpClient httpClient = _webApplicationFactory
                .WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, withdrawerId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "amount", "30" }
            };
            var path = QueryHelpers.AddQueryString("api/donate/withdraw", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
