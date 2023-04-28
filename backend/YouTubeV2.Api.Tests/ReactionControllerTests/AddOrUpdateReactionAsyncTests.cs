using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.DTO.ReactionDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.ReactionControllerTests
{
    [TestClass]
    public class AddOrUpdateReactionAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        private readonly User _user = new()
        {
            Email = "test@mail.com",
            UserName = "testUsername",
            Name = "testName",
            Surname = "testSurname",
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
        public async Task AddReactionAsync_WithReactionNotBeingNone_WhenPreviousReactionDoesNotExist_ShouldAddReaction()
        {
            // ARRANGE
            AddReactionDto addReactionDto = new (ReactionType.Positive);
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"video-reaction?id={_videoId}", new StringContent(JsonConvert.SerializeObject(addReactionDto), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Reaction? reaction = await context
                        .Reactions
                        .Include(reaction => reaction.Video)
                        .Include(reaction => reaction.User)
                        .FirstOrDefaultAsync();

                    reaction.Should().NotBeNull();
                    reaction!.ReactionType.Should().Be(addReactionDto.value);
                    reaction.Video.Id.Should().Be(_videoId);
                    reaction.User.Id.Should().Be(_userId);
                });
        }

        [TestMethod]
        public async Task AddReactionAsync_WithReactionBeingNone_WhenPreviousReactionDoesNotExist_ShouldNotAddReaction()
        {
            // ARRANGE
            AddReactionDto addReactionDto = new(ReactionType.None);
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"video-reaction?id={_videoId}", new StringContent(JsonConvert.SerializeObject(addReactionDto), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Reaction? reaction = await context.Reactions.FirstOrDefaultAsync();

                    reaction.Should().BeNull();
                });
        }

        [TestMethod]
        public async Task AddReactionAsync_WithReactionNotBeingNone_WhenPreviousReactionExists_ShouldUpdateReaction()
        {
            // ARRANGE
            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Videos.Attach(_video);
                    context.Users.Attach(_user);
                    await context.Reactions.AddAsync(new Reaction(ReactionType.Negative, _user, _video));
                    await context.SaveChangesAsync();
                });

            AddReactionDto addReactionDto = new(ReactionType.Positive);
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"video-reaction?id={_videoId}", new StringContent(JsonConvert.SerializeObject(addReactionDto), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Reaction? reaction = await context
                        .Reactions
                        .Include(reaction => reaction.Video)
                        .Include(reaction => reaction.User)
                        .FirstOrDefaultAsync();

                    reaction.Should().NotBeNull();
                    reaction!.ReactionType.Should().Be(addReactionDto.value);
                    reaction.Video.Id.Should().Be(_videoId);
                    reaction.User.Id.Should().Be(_userId);
                });
        }

        [TestMethod]
        public async Task AddReactionAsync_WithReactionBeingNone_WhenPreviousReactionExists_ShouldDeleteReaction()
        {
            // ARRANGE
            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Videos.Attach(_video);
                    context.Users.Attach(_user);
                    await context.Reactions.AddAsync(new Reaction(ReactionType.Negative, _user, _video));
                    await context.SaveChangesAsync();
                });

            AddReactionDto addReactionDto = new(ReactionType.None);
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"video-reaction?id={_videoId}", new StringContent(JsonConvert.SerializeObject(addReactionDto), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Reaction? reaction = await context.Reactions.FirstOrDefaultAsync();

                    reaction.Should().BeNull();
                });
        }
    }
}
