using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using YouTubeV2.Api.Enums;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Jobs;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class UploadVideoAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IVideoProcessingService> _videoProcessingServiceMock = new();
        private readonly static User _user = new()
        {
            Email = "test@mail.com",
            UserName = "testUsername",
            Name = "testName",
            Surname = "testSurname",
        };
        private readonly static Video _video = new()
        {
            Title = "test title",
            Description = "test description",
            Visibility = Visibility.Public,
            UploadDate = DateTimeOffset.UtcNow,
            EditDate = DateTimeOffset.UtcNow,
            Author = _user,
        };

        [TestInitialize]
        public async Task Initialize()
        {
            _videoProcessingServiceMock.Setup(x => x.EnqueVideoProcessingJobAsync(It.IsAny<VideoProcessJob>())).Returns(ValueTask.CompletedTask).Verifiable();
            _webApplicationFactory = Setup.GetWebApplicationFactoryWithVideoProcessingServiceMocked(_videoProcessingServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task UploadingUnsuportedVideoExtensionFlashShouldReturnBadRequest()
        {
            // ARRANGE
            const string videoExtension = ".flv";
            FormFile formFile = new(Stream.Null, 0, 0, "file", $"file{videoExtension}")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/x-flv",
            };

            using var multipartFormDataContent = new MultipartFormDataContent();
            using var streamContent = new StreamContent(formFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            multipartFormDataContent.Add(streamContent, "videoFile", formFile.FileName);

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccess(Role.Creator)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"api/video/{It.IsAny<Guid>()}", multipartFormDataContent);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            responseBody.Should().Be($"Video extension provided ({videoExtension}) is not supported. Supported extensions: .mkv, .mp4, .avi, .webm");
        }

        [TestMethod]
        public async Task UploadingToNonExistingVideoMetadataShouldReturnNotFound()
        {
            // ARRANGE
            FormFile formFile = new(Stream.Null, 0, 0, "file", $"file.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4",
            };

            using var multipartFormDataContent = new MultipartFormDataContent();
            using var streamContent = new StreamContent(formFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            multipartFormDataContent.Add(streamContent, "videoFile", formFile.FileName);

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccess(Role.Creator)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"api/video/{It.IsAny<Guid>()}", multipartFormDataContent);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            responseBody.Should().Be($"Video with id {It.IsAny<Guid>()} not found");
        }

        [TestMethod]
        public async Task UploadingVideoThatIsNotOwnedByUploaderShouldReturnForbidden()
        {
            // ARRANGE
            Guid videoId = Guid.Empty;
            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    var video = await context.Videos.AddAsync(_video);
                    videoId = video.Entity.Id;
                    await context.SaveChangesAsync();
                });

            FormFile formFile = new(Stream.Null, 0, 0, "file", $"file.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4",
            };

            using var multipartFormDataContent = new MultipartFormDataContent();
            using var streamContent = new StreamContent(formFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            multipartFormDataContent.Add(streamContent, "videoFile", formFile.FileName);

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, Guid.NewGuid().ToString())).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"api/video/{videoId}", multipartFormDataContent);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task UploadingVideoThatHasInvalidProcessingStateForUploadingReturnBadRequest()
        {
            // ARRANGE
            Guid videoId = Guid.Empty;
            string userId = null!;
            Video videoToAdd = new()
            {
                Title = "test title",
                Description = "test description",
                Visibility = Visibility.Public,
                UploadDate = DateTimeOffset.UtcNow,
                EditDate = DateTimeOffset.UtcNow,
                Author = _user,
                ProcessingProgress = ProcessingProgress.Ready,
            };

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    var video = await context.Videos.AddAsync(videoToAdd);
                    videoId = video.Entity.Id;
                    userId = video.Entity.Author.Id;
                    await context.SaveChangesAsync();
                });

            FormFile formFile = new(Stream.Null, 0, 0, "file", $"file.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4",
            };

            using var multipartFormDataContent = new MultipartFormDataContent();
            using var streamContent = new StreamContent(formFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            multipartFormDataContent.Add(streamContent, "videoFile", formFile.FileName);

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"api/video/{videoId}", multipartFormDataContent);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            responseBody.Should().Be($"Trying to upload video which has processing progress {videoToAdd.ProcessingProgress}");
        }

        [TestMethod]
        public async Task UploadingVideoWhenEverythingIsCorrectReturnsAccepted()
        {
            // ARRANGE
            Guid videoId = Guid.Empty;
            string userId = null!;
            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    var video = await context.Videos.AddAsync(_video);
                    videoId = video.Entity.Id;
                    userId = video.Entity.Author.Id;
                    await context.SaveChangesAsync();
                });

            FormFile formFile = new(Stream.Null, 0, 0, "file", $"file.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4",
            };

            using var multipartFormDataContent = new MultipartFormDataContent();
            using var streamContent = new StreamContent(formFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            multipartFormDataContent.Add(streamContent, "videoFile", formFile.FileName);

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"api/video/{videoId}", multipartFormDataContent);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Accepted);
            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Video? video = await context.Videos.FindAsync(videoId);
                    video.Should().NotBeNull();
                    video!.ProcessingProgress.Should().Be(ProcessingProgress.Uploading);
                });
            _videoProcessingServiceMock.Verify(x => x.EnqueVideoProcessingJobAsync(It.IsAny<VideoProcessJob>()), Times.Once);
        }
    }
}
