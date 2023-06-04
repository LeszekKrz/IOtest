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
using YouTubeV2.Application.DTO.CommentsDTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Api.Tests.CommentControllerTests
{
    [TestClass]
    public class GetCommentsAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        private readonly Mock<IBlobImageService> _blobImageServiceMock = new();

        private readonly static User _videoAuthor = new()
        {
            Email = "videoAuthor@mail.com",
            UserName = "videoAuthorUsername",
            Name = "videoAuthorName",
            Surname = "videoAuthorSurname",
        };

        private readonly static User _commentAuthor1 = new()
        {
            Email = "commentAuthor1@mail.com",
            UserName = "comAuth1Username",
            Name = "comAuth1Name",
            Surname = "comAuth1Surname",
        };

        private readonly static User _commentAuthor2 = new()
        {
            Email = "commentAuthor2@mail.com",
            UserName = "comAuth2Username",
            Name = "comAuth2Name",
            Surname = "comAuth2Surname",
        };

        private readonly static User _responseAuthor = new()
        {
            Email = "responseAuthor@mail.com",
            UserName = "respAuthUsername",
            Name = "respAuthName",
            Surname = "respAuthSurname",
        };

        private static readonly Comment _comment1 = new()
        {
            Content = "comment1 content",
            CreateDate = DateTimeOffset.UtcNow,
            Author = _commentAuthor1,
            Responses = new List<CommentResponse>()
            {
                new CommentResponse()
                {
                    Content = "comment1 response",
                    CreateDate = DateTimeOffset.UtcNow,
                    Author = _responseAuthor,
                },
            },
        };

        private static readonly Comment _comment2 = new()
        {
            Content = "comment2 content",
            CreateDate = DateTimeOffset.UtcNow,
            Author = _commentAuthor2,
        };

        private readonly static Video _video = new()
        {
            Title = "test title",
            Description = "test description",
            Visibility = Visibility.Public,
            UploadDate = DateTimeOffset.UtcNow,
            EditDate = DateTimeOffset.UtcNow,
            Author = _videoAuthor,
            Comments = new List<Comment>()
            {
                _comment1,
                _comment2,
            },
        };

        private Guid _videoId;

        private Guid _comment1Id;

        private Guid _comment2Id;

        private readonly static Video _otherVideo = new()
        {
            Title = "other test title",
            Description = "other test description",
            Visibility = Visibility.Public,
            UploadDate = DateTimeOffset.UtcNow,
            EditDate = DateTimeOffset.UtcNow,
            Author = _videoAuthor,
            Comments = new List<Comment>()
            {
                new Comment()
                {
                    Content = "other comment1 content",
                    CreateDate = DateTimeOffset.UtcNow,
                    Author = _commentAuthor1,
                    Responses = new List<CommentResponse>()
                    {
                        new CommentResponse()
                        {
                            Content = "other comment1 response",
                            CreateDate = DateTimeOffset.UtcNow,
                            Author = _responseAuthor,
                        },
                    },
                },
                new Comment()
                {
                    Content = "other comment2 content",
                    CreateDate = DateTimeOffset.UtcNow,
                    Author = _commentAuthor2,
                },
            },
        };

        private const string _profilePicuteDomain = "http://www.profile-picture/";
        

        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageServiceMock.Setup(x => x.GetProfilePictureUrl(It.IsAny<string>())).Returns<string>(fileName => new Uri($"{_profilePicuteDomain}{fileName}"));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_videoAuthor);
                  await userManager.CreateAsync(_commentAuthor1);
                  await userManager.CreateAsync(_commentAuthor2);
                  await userManager.CreateAsync(_responseAuthor);
              });

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_videoAuthor);
                    context.Users.Attach(_commentAuthor1);
                    context.Users.Attach(_commentAuthor2);
                    context.Users.Attach(_responseAuthor);
                    var video = await context.Videos.AddAsync(_video);
                    await context.Videos.AddAsync(_otherVideo);
                    await context.SaveChangesAsync();
                    _videoId = video.Entity.Id;
                    _comment1Id = video.Entity.Comments.Single(comment => comment.Content == _comment1.Content).Id;
                    _comment2Id = video.Entity.Comments.Single(comment => comment.Content == _comment2.Content).Id;
                });
        }

        [TestMethod]
        public async Task GetCommentsAsync_ShouldReturnAllComments()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccess(Role.Creator)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"api/comment?id={_videoId}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<CommentsDTO>(responseBody);
            deserializedResponseBody.comments.Count.Should().Be(2);
            deserializedResponseBody.comments.Should().BeEquivalentTo(new CommentsDTO.CommentDTO[]
            {
                new (_comment1Id, _comment1.Author.Id, _comment1.Content, new Uri($"{_profilePicuteDomain}{_comment1.Author.Id}"), _comment1.Author.UserName!, true),
                new (_comment2Id, _comment2.Author.Id, _comment2.Content, new Uri($"{_profilePicuteDomain}{_comment2.Author.Id}"), _comment2.Author.UserName!, false),
            });
        }
    }
}
