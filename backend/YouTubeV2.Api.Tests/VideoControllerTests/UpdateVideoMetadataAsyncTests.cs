using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Api.Tests.Providers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Net;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class UpdateVideoMetadataAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IBlobImageService> _blobImageService = new();
        private readonly User _user = new()
        {
            Email = "test@mail.com",
            UserName = "testUsername",
            Name = "testName",
            Surname = "testSurname",
        };
        private string _userId = null!;
        private Guid _videoId;

        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageService.Setup(x => x.UploadVideoThumbnailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageService.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  _userId = await userManager.GetUserIdAsync(_user);
              });

            Video video = new(
                "OldTitle",
                "OldDescription",
                Visibility.Private,
                new[] { "OldTag1", "OldTag2" },
                _user,
                DateTimeOffset.UtcNow);

            await _webApplicationFactory.DoWithinScope<YTContext>(
              async context =>
              {
                  context.Attach(_user);
                  var videoEntity = await context.AddAsync(video);
                  await context.SaveChangesAsync();
                  _videoId = videoEntity.Entity.Id;
              });
        }

        [TestMethod]
        public async Task UpdateVideoMetadataAsync_ShouldUpdateTheMetadata()
        {
            // ARRANGE
            VideoMetadataAddOrUpdateDto videoMetadata = new(
                "NewTitle",
                "NewDescription",
                "data:image/png;base64,iVBORw0KGg==",
                new[] { "NewTag1", "NewTag2" },
                Visibility.Public);

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PutAsync($"api/video-metadata?id={_videoId}", new StringContent(JsonConvert.SerializeObject(videoMetadata), Encoding.UTF8, "application/json"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
              async context =>
              {
                  Video? video = await context
                    .Videos
                    .Include(video => video.Tags)
                    .FirstOrDefaultAsync(video => video.Id == _videoId);

                  video.Should().NotBeNull();
                  video!.Title.Should().Be(videoMetadata.title);
                  video.Description.Should().Be(videoMetadata.description);
                  video.Visibility.Should().Be(videoMetadata.visibility);
                  video.Tags.Select(tag => tag.Value).Should().BeEquivalentTo(videoMetadata.tags);
              });

            _blobImageService.Verify(x => x.UploadVideoThumbnailAsync(videoMetadata.thumbnail, _videoId.ToString(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
