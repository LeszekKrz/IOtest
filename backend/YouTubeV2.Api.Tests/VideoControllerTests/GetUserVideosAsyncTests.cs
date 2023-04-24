using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Net;
using YouTubeV2.Api.Enums;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class GetUserVideosAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        private readonly Mock<IBlobImageService> _blobImageServiceMock = new();

        private static readonly DateTimeOffset _firstUploadTime = DateTimeOffset.UtcNow;
        
        private static readonly DateTimeOffset _secondUploadTime = DateTimeOffset.UtcNow.AddDays(1);

        private static readonly DateTimeOffset _thirdUploadTime = DateTimeOffset.UtcNow.AddDays(2);

        private static User _user = new()
        {
            Email = "user@mail.com",
            UserName = "userUsername",
            Name = "userName",
            Surname = "userSurname",
        };

        private static User _otherUser = new()
        {
            Email = "other@mail.com",
            UserName = "otherUsername",
            Name = "otherName",
            Surname = "otherSurname",
        };

        private static Video _userPublicReadyVideo = new()
        {
            Title = "public ready title",
            Description = "public ready description",
            Visibility = Visibility.Public,
            UploadDate = _firstUploadTime,
            EditDate = _firstUploadTime,
            ProcessingProgress = ProcessingProgress.Ready,
            Author = _user,
            Duration = "10:10",
        };

        private static Video _userPrivateReadyVideo = new()
        {
            Title = "private ready title",
            Description = "private ready description",
            Visibility = Visibility.Private,
            UploadDate = _secondUploadTime,
            EditDate = _secondUploadTime,
            ProcessingProgress = ProcessingProgress.Ready,
            Author = _user,
            Duration = "20:20",
        };

        private static Video _userPublicNotReadyVideo = new()
        {
            Title = "public not ready title",
            Description = "public not ready description",
            Visibility = Visibility.Public,
            UploadDate = _thirdUploadTime,
            EditDate = _thirdUploadTime,
            ProcessingProgress = ProcessingProgress.Processing,
            Author = _user,
            Duration = "30:30",
        };

        private static Video _otherUserPublicReadyVideo = new()
        {
            Title = "other user private ready title",
            Description = "other user private ready description",
            Visibility = Visibility.Public,
            UploadDate = _firstUploadTime,
            EditDate = _firstUploadTime,
            ProcessingProgress = ProcessingProgress.Ready,
            Author = _otherUser,
            Duration = "40:40",
        };

        private const string _videoThumbnailDomain = "http://www.profile-picture/";

        private string _userId = null!;

        private string _otherUserId = null!;

        private Guid _userPublicReadyVideoId;

        private Guid _userPrivateReadyVideoId;

        private Guid _userPublicNotReadyVideoId;


        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageServiceMock.Setup(x => x.GetVideoThumbnail(It.IsAny<string>())).Returns<string>(fileName => new Uri($"{_videoThumbnailDomain}{fileName.ToLower()}"));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    var userPublicReadyVideo = await context.Videos.AddAsync(_userPublicReadyVideo);
                    var userPrivateReadyVideo = await context.Videos.AddAsync(_userPrivateReadyVideo);
                    var userPublicNotReadyVideo = await context.Videos.AddAsync(_userPublicNotReadyVideo);
                    var otherUserPublicReadyVideo = await context.Videos.AddAsync(_otherUserPublicReadyVideo);
                    await context.SaveChangesAsync();
                    _userPublicReadyVideoId = userPublicReadyVideo.Entity.Id;
                    _userPrivateReadyVideoId = userPrivateReadyVideo.Entity.Id;
                    _userPublicNotReadyVideoId = userPublicNotReadyVideo.Entity.Id;
                    _userId = userPublicReadyVideo.Entity.Author.Id;
                    _otherUserId = otherUserPublicReadyVideo.Entity.Author.Id;
                });
        }

        [TestMethod]
        public async Task GetUserVideoAsync_WhenUserIdFromAuthenticationIsTheSameAsUserIdFromRequest_ReturnsAllUserVideos()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"user/videos?id={_userId}");

            // ARRANGE
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<VideoListDto>(responseBody);
            deserializedResponseBody.Should().NotBeNull();
            deserializedResponseBody.videos.Count.Should().Be(3);
            deserializedResponseBody.videos.Should().BeEquivalentTo(new[] {
                new VideoMetadataDto(
                    _userPublicNotReadyVideoId,
                    _userPublicNotReadyVideo.Title,
                    _userPublicNotReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPublicNotReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPublicNotReadyVideo.Visibility,
                    _userPublicNotReadyVideo.ProcessingProgress,
                    _userPublicNotReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicNotReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicNotReadyVideo.Duration),
                new VideoMetadataDto(
                    _userPrivateReadyVideoId,
                    _userPrivateReadyVideo.Title,
                    _userPrivateReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPrivateReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPrivateReadyVideo.Visibility,
                    _userPrivateReadyVideo.ProcessingProgress,
                    _userPrivateReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPrivateReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPrivateReadyVideo.Duration),
                new VideoMetadataDto(
                    _userPublicReadyVideoId,
                    _userPublicReadyVideo.Title,
                    _userPublicReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPublicReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPublicReadyVideo.Visibility,
                    _userPublicReadyVideo.ProcessingProgress,
                    _userPublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicReadyVideo.Duration),
            }, options => options.WithStrictOrdering());
        }

        [TestMethod]
        public async Task GetUserVideoAsync_WhenUserIdFromRequestIsNull_ReturnsAllUserVideos()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"user/videos");

            // ARRANGE
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<VideoListDto>(responseBody);
            deserializedResponseBody.Should().NotBeNull();
            deserializedResponseBody.videos.Count.Should().Be(3);
            deserializedResponseBody.videos.Should().BeEquivalentTo(new[] {
                new VideoMetadataDto(
                    _userPublicNotReadyVideoId,
                    _userPublicNotReadyVideo.Title,
                    _userPublicNotReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPublicNotReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPublicNotReadyVideo.Visibility,
                    _userPublicNotReadyVideo.ProcessingProgress,
                    _userPublicNotReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicNotReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicNotReadyVideo.Duration),
                new VideoMetadataDto(
                    _userPrivateReadyVideoId,
                    _userPrivateReadyVideo.Title,
                    _userPrivateReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPrivateReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPrivateReadyVideo.Visibility,
                    _userPrivateReadyVideo.ProcessingProgress,
                    _userPrivateReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPrivateReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPrivateReadyVideo.Duration),
                new VideoMetadataDto(
                    _userPublicReadyVideoId,
                    _userPublicReadyVideo.Title,
                    _userPublicReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPublicReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPublicReadyVideo.Visibility,
                    _userPublicReadyVideo.ProcessingProgress,
                    _userPublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicReadyVideo.Duration),
            }, options => options.WithStrictOrdering());
        }

        [TestMethod]
        public async Task GetUserVideoAsync_WhenUserIdFromAuthenticationVaryFromUserIdFromRequest_ReturnsAllAvailableUserVideos()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _otherUserId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"user/videos?id={_userId}");

            // ARRANGE
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<VideoListDto>(responseBody);
            deserializedResponseBody.Should().NotBeNull();
            deserializedResponseBody.videos.Count.Should().Be(1);
            deserializedResponseBody.videos.Should().BeEquivalentTo(new[] {
                new VideoMetadataDto(
                    _userPublicReadyVideoId,
                    _userPublicReadyVideo.Title,
                    _userPublicReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_userPublicReadyVideoId}"),
                    _userId,
                    _user.UserName!,
                    0,
                    Array.Empty<string>(),
                    _userPublicReadyVideo.Visibility,
                    _userPublicReadyVideo.ProcessingProgress,
                    _userPublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _userPublicReadyVideo.Duration),
            });
        }
    }
}
