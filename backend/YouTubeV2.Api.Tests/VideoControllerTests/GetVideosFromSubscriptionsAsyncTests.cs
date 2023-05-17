using FluentAssertions;
using Microsoft.AspNetCore.Identity;
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
    public class GetVideosFromSubscriptionsAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        private readonly Mock<IBlobImageService> _blobImageServiceMock = new();

        private static readonly DateTimeOffset _firstUploadTime = DateTimeOffset.UtcNow;

        private static readonly DateTimeOffset _secondUploadTime = DateTimeOffset.UtcNow.AddDays(1);

        private static readonly DateTimeOffset _thirdUploadTime = DateTimeOffset.UtcNow.AddDays(2);

        private static readonly User _creator1 = new()
        {
            Email = "creator1@mail.com",
            UserName = "creator1Username",
            Name = "creator1Name",
            Surname = "creator1Surname",
        };

        private static readonly User _creator2 = new()
        {
            Email = "creator2@mail.com",
            UserName = "creator2Username",
            Name = "creator2Name",
            Surname = "creator2Surname",
        };

        private static readonly User _user = new()
        {
            Email = "user@mail.com",
            UserName = "userUsername",
            Name = "userName",
            Surname = "userSurname",
        };

        private readonly Video _creator1PublicReadyVideo1 = new()
        {
            Title = "creator1 public ready video1 title",
            Description = "creator1 public ready video1 description",
            Visibility = Visibility.Public,
            UploadDate = _firstUploadTime,
            EditDate = _firstUploadTime,
            Author = _creator1,
            ProcessingProgress = ProcessingProgress.Ready,
        };

        private readonly Video _creator1PublicReadyVideo2 = new()
        {
            Title = "creator1 public ready video2 title",
            Description = "creator1 public ready video2 description",
            Visibility = Visibility.Public,
            UploadDate = _thirdUploadTime,
            EditDate = _thirdUploadTime,
            Author = _creator1,
            ProcessingProgress = ProcessingProgress.Ready,
        };

        private readonly Video _creator1PrivateReadyVideo = new()
        {
            Title = "creator1 private ready video title",
            Description = "creator1 private ready video description",
            Visibility = Visibility.Private,
            UploadDate = _firstUploadTime,
            EditDate = _firstUploadTime,
            Author = _creator1,
            ProcessingProgress = ProcessingProgress.Ready,
        };

        private readonly Video _creator1PublicNotReadyVideo2 = new()
        {
            Title = "creator1 public not ready video title",
            Description = "creator1 public not ready video description",
            Visibility = Visibility.Public,
            UploadDate = _firstUploadTime,
            EditDate = _firstUploadTime,
            Author = _creator1,
            ProcessingProgress = ProcessingProgress.Processing,
        };

        private readonly Video _creator2PublicReadyVideo = new()
        {
            Title = "creator2 public ready video title",
            Description = "creator2 public ready video description",
            Visibility = Visibility.Public,
            UploadDate = _secondUploadTime,
            EditDate = _secondUploadTime,
            Author = _creator2,
            ProcessingProgress = ProcessingProgress.Ready,
        };

        private string _creator1Id = null!;
        private string _creator2Id = null!;
        private string _userId = null!;
        private Guid _creator1PublicReadyVideo1Id;
        private Guid _creator1PublicReadyVideo2Id;
        private Guid _creator2PublicReadyVideoId;

        private const string _videoThumbnailDomain = "http://www.profile-picture/";

        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageServiceMock.Setup(x => x.GetVideoThumbnailUrl(It.IsAny<string>())).Returns<string>(fileName => new Uri($"{_videoThumbnailDomain}{fileName.ToLower()}"));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_creator1);
                  await userManager.CreateAsync(_creator2);
                  await userManager.CreateAsync(_user);
                  _creator1Id = await userManager.GetUserIdAsync(_creator1);
                  _creator2Id = await userManager.GetUserIdAsync(_creator2);
                  _userId = await userManager.GetUserIdAsync(_user);
              });

            Subscription[] subscriptions = new[]
            {
                new Subscription()
                {
                    Subscribee = _creator1,
                    SubscribeeId = _creator1Id,
                    Subscriber = _user,
                    SubscriberId = _userId
                },
                new Subscription()
                {
                    Subscribee = _creator2,
                    SubscribeeId = _creator2Id,
                    Subscriber = _user,
                    SubscriberId = _userId
                },
            };

            Video[] videos = new[]
            {
                _creator1PublicReadyVideo1,
                _creator1PublicReadyVideo2,
                _creator1PrivateReadyVideo,
                _creator1PublicNotReadyVideo2,
                _creator2PublicReadyVideo,
            };

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_creator1);
                    context.Users.Attach(_creator2);
                    context.Users.Attach(_user);
                    await context.Subscriptions.AddRangeAsync(subscriptions);
                    var creator1PublicReadyVideo1 = await context.Videos.AddAsync(_creator1PublicReadyVideo1);
                    var creator1PublicReadyVideo2 = await context.Videos.AddAsync(_creator1PublicReadyVideo2);
                    await context.Videos.AddAsync(_creator1PrivateReadyVideo);
                    await context.Videos.AddAsync(_creator1PublicNotReadyVideo2);
                    var creator2PublicReadyVideo = await context.Videos.AddAsync(_creator2PublicReadyVideo);
                    await context.SaveChangesAsync();
                    _creator1PublicReadyVideo1Id = creator1PublicReadyVideo1.Entity.Id;
                    _creator1PublicReadyVideo2Id = creator1PublicReadyVideo2.Entity.Id;
                    _creator2PublicReadyVideoId = creator2PublicReadyVideo.Entity.Id;
                });
        }

        [TestMethod]
        public async Task GetVideosFromSubscriptionsAsync_ReturnsAllVideosFromUserSubscriptions()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, _userId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"user/videos/subscribed");

            // ARRANGE
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<VideoListDto>(responseBody);
            deserializedResponseBody.Should().NotBeNull();
            deserializedResponseBody.videos.Count.Should().Be(3);
            deserializedResponseBody.videos.Should().BeEquivalentTo(new[] {
                new VideoMetadataDto(
                    _creator1PublicReadyVideo2Id,
                    _creator1PublicReadyVideo2.Title,
                    _creator1PublicReadyVideo2.Description,
                    new Uri($"{_videoThumbnailDomain}{_creator1PublicReadyVideo2Id}"),
                    _creator1Id,
                    _creator1.UserName!,
                    0,
                    Array.Empty<string>(),
                    _creator1PublicReadyVideo2.Visibility,
                    _creator1PublicReadyVideo2.ProcessingProgress,
                    _creator1PublicReadyVideo2.UploadDate.ToUniversalTime().Date,
                    _creator1PublicReadyVideo2.UploadDate.ToUniversalTime().Date,
                    _creator1PublicReadyVideo2.Duration),
                new VideoMetadataDto(
                    _creator2PublicReadyVideoId,
                    _creator2PublicReadyVideo.Title,
                    _creator2PublicReadyVideo.Description,
                    new Uri($"{_videoThumbnailDomain}{_creator2PublicReadyVideoId}"),
                    _creator2Id,
                    _creator2.UserName!,
                    0,
                    Array.Empty<string>(),
                    _creator2PublicReadyVideo.Visibility,
                    _creator2PublicReadyVideo.ProcessingProgress,
                    _creator2PublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _creator2PublicReadyVideo.UploadDate.ToUniversalTime().Date,
                    _creator2PublicReadyVideo.Duration),
                new VideoMetadataDto(
                    _creator1PublicReadyVideo1Id,
                    _creator1PublicReadyVideo1.Title,
                    _creator1PublicReadyVideo1.Description,
                    new Uri($"{_videoThumbnailDomain}{_creator1PublicReadyVideo1Id}"),
                    _creator1Id,
                    _creator1.UserName!,
                    0,
                    Array.Empty<string>(),
                    _creator1PublicReadyVideo1.Visibility,
                    _creator1PublicReadyVideo1.ProcessingProgress,
                    _creator1PublicReadyVideo1.UploadDate.ToUniversalTime().Date,
                    _creator1PublicReadyVideo1.UploadDate.ToUniversalTime().Date,
                    _creator1PublicReadyVideo1.Duration),
            }, options => options.WithStrictOrdering());
        }
    }
}
