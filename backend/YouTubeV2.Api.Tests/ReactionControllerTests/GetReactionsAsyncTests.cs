using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.DTO.ReactionDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.ReactionControllerTests
{
    [TestClass]
    public class GetReactionsAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        private readonly User _user = new()
        {
            Email = "user@mail.com",
            UserName = "userUsername",
            Name = "userName",
            Surname = "userSurname",
        };

        private readonly User _otherUser1 = new()
        {
            Email = "otherUser1@mail.com",
            UserName = "otherUser1Username",
            Name = "otherUser1Name",
            Surname = "othreUser1Surname",
        };

        private readonly User _otherUser2 = new()
        {
            Email = "otherUser2@mail.com",
            UserName = "otherUser2Username",
            Name = "otherUser2Name",
            Surname = "othreUser2Surname",
        };

        private string _userId = null!;

        private Video _video = null!;

        private Guid _videoId;

        [TestInitialize]
        public async Task Initialize()
        {
            _video = new()
            {
                Title = "test title",
                Description = "test description",
                Author = _user,
            };

            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  _userId = await userManager.GetUserIdAsync(_user);
              });

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_user);
                    var video = await context.Videos.AddAsync(_video);
                    await context.SaveChangesAsync();
                    _videoId = video.Entity.Id;
                });
        }

        [TestMethod]
        public async Task GetReactionsAsync_WhenThereIsNoReactions_ShouldReturnCorrectly()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"video-reaction?id={_videoId}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var reactions = JsonConvert.DeserializeObject<ReactionsDto>(content);

            reactions.positiveCount.Should().Be(0);
            reactions.negativeCount.Should().Be(0);
            reactions.currentUserReaction.Should().Be(ReactionType.None);
        }

        [TestMethod]
        public async Task GetReactionsAsync_WhenThereIsReactions_ShouldReturnCorrectly()
        {
            // ARRANGE
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_otherUser1);
                  await userManager.CreateAsync(_otherUser2);
              });

            Reaction userReaction = new Reaction(ReactionType.Positive, _user, _video);
            Reaction otherUser1Reaction = new Reaction(ReactionType.Positive, _otherUser1, _video);
            Reaction otherUser2Reaction = new Reaction(ReactionType.Negative, _otherUser2, _video);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_user);
                    context.Users.Attach(_otherUser1);
                    context.Users.Attach(_otherUser2);
                    context.Videos.Attach(_video);
                    await context.Reactions.AddRangeAsync(new[] { userReaction, otherUser1Reaction, otherUser2Reaction });
                    await context.SaveChangesAsync();
                });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"video-reaction?id={_videoId}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var reactions = JsonConvert.DeserializeObject<ReactionsDto>(content);

            reactions.positiveCount.Should().Be(2);
            reactions.negativeCount.Should().Be(1);
            reactions.currentUserReaction.Should().Be(ReactionType.Positive);
        }
    }
}
