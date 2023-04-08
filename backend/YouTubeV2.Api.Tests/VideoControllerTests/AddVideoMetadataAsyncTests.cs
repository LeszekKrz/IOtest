using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Moq;
using YouTubeV2.Application.Services;
using System.Net;
using FluentAssertions;
using YouTubeV2.Api.Tests.Providers;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Providers;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class AddVideoMetadataAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
        private readonly User _user = new()
        {
            Email = "test@mail.com",
            UserName = "testUsername",
            Name = "testName",
            Surname = "testSurname",
        };
        private readonly DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

        [TestInitialize]
        public async Task Initialize()
        {
            _userServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(_user);
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);
            _webApplicationFactory = Setup.GetWebApplicationFactory(_userServiceMock.Object, _dateTimeProviderMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task AddVideoMetadataAsync_ValidInput_ReturnsOk()
        {
            // ARRANGE
            VideoMetadataPostDTO videoMetadata = new(
            "Test Video Title",
                "Test Video Description",
                "data:image/png;base64,iVBORw0KGg==",
                new[] { "test tag1", "test tag2" },
                Visibility.Public);

            string userId = null!;
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  userId = await userManager.GetUserIdAsync(_user);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithCreatorAccessAndUserId(userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync("video-metadata", new StringContent(JsonConvert.SerializeObject(videoMetadata), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<VideoMetadataPostResponseDTO>(responseBody);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Video? videoResult = await context
                    .Videos
                    .Include(video => video.User)
                    .Include(video => video.Tags)
                    .SingleOrDefaultAsync();

                    videoResult.Should().NotBeNull();
                    videoResult!.Id.Should().Be(deserializedResponseBody.id);
                    videoResult!.Title.Should().Be(videoMetadata.title);
                    videoResult!.Description.Should().Be(videoMetadata.description);
                    videoResult!.Visibility.Should().Be(videoMetadata.visibility);
                    videoResult!.ViewCount.Should().Be(0);
                    videoResult!.ProcessingProgress.Should().Be(ProcessingProgress.MetadataRecordCreater);
                    videoResult!.UploadDate.Should().Be(_utcNow);
                    videoResult!.EditDate.Should().Be(_utcNow);
                    videoResult!.Duration.Should().Be("00:00");
                    videoResult.User.Id.Should().Be(userId);
                    videoResult!.Tags.Select(tag => tag.Value).Should().BeEquivalentTo(videoMetadata.tags);
                });
        }

        [TestMethod]
        public async Task AddVideoMetadataAsyncBeingSimpleUser_ReturnsForbidden()
        {
            // ARRANGE
            VideoMetadataPostDTO videoMetadata = new(
            "Test Video Title",
                "Test Video Description",
                "data:image/png;base64,iVBORw0KGg==",
                new[] { "test tag1", "test tag2" },
                Visibility.Public);

            string userId = null!;
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  userId = await userManager.GetUserIdAsync(_user);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithSimpleAccessAndUserId(userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync("video-metadata", new StringContent(JsonConvert.SerializeObject(videoMetadata), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task AddVideoMetadataAsyncWithoutBeingAuthorized_ReturnsUnauthorized()
        {
            // ARRANGE
            VideoMetadataPostDTO videoMetadata = new(
            "Test Video Title",
                "Test Video Description",
                "data:image/png;base64,iVBORw0KGg==",
                new[] { "test tag1", "test tag2" },
                Visibility.Public);

            string userId = null!;
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  userId = await userManager.GetUserIdAsync(_user);
              });

            using HttpClient httpClient = _webApplicationFactory.CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync("video-metadata", new StringContent(JsonConvert.SerializeObject(videoMetadata), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
