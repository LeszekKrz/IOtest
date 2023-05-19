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
    public class SendDonationAsyncTests
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
        public async Task SendDonation_SimpleToCreator_GoodAmmount_ShouldChangeBallance()
        {
            // ARRANGE
            User sender = new User("sender1@mail.com", "sender1", "sender1", "sender1");
            User reciever = new User("reciever1@mail.com", "reciever1", "reciever1", "reciever1");

            sender.AccountBalance = 20;
            reciever.AccountBalance = 10;

            string senderId = null!;
            string recieverId = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(sender);
                    await userManager.CreateAsync(reciever);

                    await userManager.AddToRoleAsync(sender, Role.Simple);
                    await userManager.AddToRoleAsync(reciever, Role.Creator);

                    senderId = await userManager.GetUserIdAsync(sender);
                    recieverId = await userManager.GetUserIdAsync(reciever);
                });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, senderId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "id", recieverId },
                { "amount", "10" }
            };
            var path = QueryHelpers.AddQueryString("donate/send", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    var senderFound = await userManager.FindByIdAsync(senderId);
                    var recieverFound = await userManager.FindByIdAsync(recieverId);

                    senderFound!.AccountBalance.Should().Be(10);
                    recieverFound!.AccountBalance.Should().Be(20);
                });
        }

        [TestMethod]
        public async Task SendDonation_SimpleToCreator_NegativeAmmount_ShouldReturnBadRequest()
        {
            // ARRANGE
            User sender = new User("sender2@mail.com", "sender2", "sender2", "sender2");
            User reciever = new User("reciever2@mail.com", "reciever2", "reciever2", "reciever2");

            sender.AccountBalance = 20;
            reciever.AccountBalance = 10;

            string senderId = null!;
            string recieverId = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(sender);
                    await userManager.CreateAsync(reciever);

                    await userManager.AddToRoleAsync(sender, Role.Simple);
                    await userManager.AddToRoleAsync(reciever, Role.Creator);

                    senderId = await userManager.GetUserIdAsync(sender);
                    recieverId = await userManager.GetUserIdAsync(reciever);
                });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, senderId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "id", recieverId },
                { "amount", "-10" }
            };
            var path = QueryHelpers.AddQueryString("donate/send", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task SendDonation_SimpleToCreator_NotEnoughAmmount_ShouldReturnBadRequest()
        {
            // ARRANGE
            User sender = new User("sender3@mail.com", "sender3", "sender3", "sender3");
            User reciever = new User("reciever3@mail.com", "reciever3", "reciever3", "reciever3");

            sender.AccountBalance = 20;
            reciever.AccountBalance = 10;

            string senderId = null!;
            string recieverId = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(sender);
                    await userManager.CreateAsync(reciever);

                    await userManager.AddToRoleAsync(sender, Role.Simple);
                    await userManager.AddToRoleAsync(reciever, Role.Creator);

                    senderId = await userManager.GetUserIdAsync(sender);
                    recieverId = await userManager.GetUserIdAsync(reciever);
                });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, senderId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "id", recieverId },
                { "amount", "30" }
            };
            var path = QueryHelpers.AddQueryString("donate/send", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task SendDonation_SimpleToSimple_GoodAmmount_ShouldReturnBadRequest()
        {
            // ARRANGE
            User sender = new User("sender4@mail.com", "sender4", "sender4", "sender4");
            User reciever = new User("reciever4@mail.com", "reciever4", "reciever4", "reciever4");

            sender.AccountBalance = 20;
            reciever.AccountBalance = 10;

            string senderId = null!;
            string recieverId = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    await userManager.CreateAsync(sender);
                    await userManager.CreateAsync(reciever);

                    await userManager.AddToRoleAsync(sender, Role.Simple);
                    await userManager.AddToRoleAsync(reciever, Role.Simple);

                    senderId = await userManager.GetUserIdAsync(sender);
                    recieverId = await userManager.GetUserIdAsync(reciever);
                });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, senderId)).CreateClient();

            // ACT
            var query = new Dictionary<string, string?>()
            {
                { "id", recieverId },
                { "amount", "10" }
            };
            var path = QueryHelpers.AddQueryString("donate/send", query);
            var response = await httpClient.PostAsync(path, null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
