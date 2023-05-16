using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using YouTubeV2.Application;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using System.Net;
using YouTubeV2.Api.Tests.Providers;
using Microsoft.EntityFrameworkCore;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class DeleteVideoAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        private readonly Mock<IBlobImageService> _blobImageService = new();

        private readonly User _author = new()
        {
            Email = "test@mail.com",
            UserName = "testUsername",
            Name = "testName",
            Surname = "testSurname",
        };

        private string _authorId = null!;
        private Guid _videoId;
        private Guid _otherVideoId;

        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageService.Setup(x => x.DeleteThumbnailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageService.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_author);
                  _authorId = await userManager.GetUserIdAsync(_author);
              });

            Video video = new()
            {
                Title = "Test title",
                Description = "Test description",
                Author = _author,
                Comments = new[]
                {
                    new Comment
                    {
                        Author = _author,
                        Content = "comment1 content",
                        Responses = new[]
                        {
                            new CommentResponse
                            {
                                Author = _author,
                                Content = "comment response1",
                            },
                            new CommentResponse
                            {
                                Author = _author,
                                Content = "comment response2",
                            }
                        }
                    },
                    new Comment
                    {
                        Author = _author,
                        Content = "comment2 content",
                    },
                }
            };

            Video otherVideo = new()
            {
                Title = "Other test title",
                Description = "Other test description",
                Author = _author,
            };

            Playlist playlist = new()
            {
                Creator = _author,
                Name = "Playlist name",
                Videos = new[] { video, otherVideo, },
            };


            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_author);
                    var videoEntity = await context.Videos.AddAsync(video);
                    var otherVideoEntity = await context.Videos.AddAsync(otherVideo);
                    await context.Playlists.AddAsync(playlist);
                    await context.SaveChangesAsync();
                    _videoId = videoEntity.Entity.Id;
                    _otherVideoId = otherVideoEntity.Entity.Id;
                });
        }

        [TestMethod]
        public async Task DeleteVideoAsync_WhenYouAreTheOwner_DeletesTheVideoAndRelatedComments()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _authorId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"video?id={_videoId}");

            // ARRANGE
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            _blobImageService.Verify(x => x.DeleteThumbnailAsync(_videoId.ToString(), It.IsAny<CancellationToken>()), Times.Once);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Video? video = await context.Videos.FindAsync(_videoId);
                    video.Should().BeNull();

                    Playlist playlist = await context.Playlists.Include(playlist => playlist.Videos).SingleAsync();
                    playlist.Videos.Should().HaveCount(1);
                    playlist.Videos.Single().Id.Should().Be(_otherVideoId);

                    int commentsCount = await context.Comments.CountAsync();
                    commentsCount.Should().Be(0);

                    int commentsResponsesCount = await context.CommentResponses.CountAsync();
                    commentsResponsesCount.Should().Be(0);
                });
        }

        [TestMethod]
        public async Task DeleteVideoAsync_WhenYouAreAdministrator_DeletesTheVideoAndRelatedComments()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Administrator, "not author id")).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"video?id={_videoId}");

            // ARRANGE
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            _blobImageService.Verify(x => x.DeleteThumbnailAsync(_videoId.ToString(), It.IsAny<CancellationToken>()), Times.Once);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Video? video = await context.Videos.FindAsync(_videoId);
                    video.Should().BeNull();

                    Playlist playlist = await context.Playlists.Include(playlist => playlist.Videos).SingleAsync();
                    playlist.Videos.Should().HaveCount(1);
                    playlist.Videos.Single().Id.Should().Be(_otherVideoId);

                    int commentsCount = await context.Comments.CountAsync();
                    commentsCount.Should().Be(0);

                    int commentsResponsesCount = await context.CommentResponses.CountAsync();
                    commentsResponsesCount.Should().Be(0);
                });
        }
    }
}
