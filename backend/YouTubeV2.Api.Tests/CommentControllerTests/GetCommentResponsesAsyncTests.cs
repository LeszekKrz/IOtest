using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Net;
using YouTubeV2.Api.Enums;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application.DTO.CommentsDTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YouTubeV2.Api.Tests.CommentControllerTests
{
    [TestClass]
    public class GetCommentResponsesAsyncTests
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

        private readonly static User _commentAuthor = new()
        {
            Email = "commentAuthor1@mail.com",
            UserName = "comAuth1Username",
            Name = "comAuth1Name",
            Surname = "comAuth1Surname",
        };

        private readonly static User _responseAuthor1 = new()
        {
            Email = "responseAuthor1@mail.com",
            UserName = "respAuth1Username",
            Name = "respAuth1Name",
            Surname = "respAuth1Surname",
        };

        private readonly static User _responseAuthor2 = new()
        {
            Email = "responseAuthor2@mail.com",
            UserName = "respAuth2Username",
            Name = "respAuth2Name",
            Surname = "respAuth2Surname",
        };

        private readonly static CommentResponse _comment1Response1 = new ()
        {
            Content = "comment1 response1",
            CreateDate = DateTimeOffset.UtcNow,
            Author = _responseAuthor1,
        };

        private readonly static CommentResponse _comment1Response2 = new ()
        {
            Content = "comment1 response2",
            CreateDate = DateTimeOffset.UtcNow,
            Author = _responseAuthor2,
        };

        private static readonly Comment _comment1 = new()
        {
            Content = "comment1 content",
            CreateDate = DateTimeOffset.UtcNow,
            Author = _commentAuthor,
            Responses = new List<CommentResponse>()
            {
                _comment1Response1,
                _comment1Response2,
            },
        };

        private static readonly Comment _comment2 = new()
        {
            Content = "comment2 content",
            CreateDate = DateTimeOffset.UtcNow,
            Author = _commentAuthor,
            Responses = new List<CommentResponse>
            {
                new CommentResponse()
                {
                    Content = "comment2 response1",
                    CreateDate = DateTimeOffset.UtcNow,
                    Author = _responseAuthor1,
                },
            },
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

        private Guid _comment1Id;

        private Guid _comment1Response1Id;

        private Guid _comment1Response2Id;

        private const string _profilePicuteDomain = "http://www.profile-picture/";


        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageServiceMock.Setup(x => x.GetProfilePicture(It.IsAny<string>())).Returns<string>(fileName => new Uri($"{_profilePicuteDomain}{fileName}"));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_videoAuthor);
                  await userManager.CreateAsync(_commentAuthor);
                  await userManager.CreateAsync(_responseAuthor1);
                  await userManager.CreateAsync(_responseAuthor2);
              });

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_videoAuthor);
                    context.Users.Attach(_commentAuthor);
                    context.Users.Attach(_responseAuthor1);
                    context.Users.Attach(_responseAuthor2);
                    var video = await context.Videos.AddAsync(_video);
                    await context.SaveChangesAsync();
                    var comment1 = video.Entity.Comments.Single(comment => comment.Content == _comment1.Content);
                    _comment1Id = comment1.Id;
                    _comment1Response1Id = comment1.Responses.Single(response => response.Content == _comment1Response1.Content).Id;
                    _comment1Response2Id = comment1.Responses.Single(response => response.Content == _comment1Response2.Content).Id;
                });
        }

        [TestMethod]
        public async Task GetCommentsAsync_ShouldReturnAllComments()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccess(Role.Creator)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.GetAsync($"comment/response?id={_comment1Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var deserializedResponseBody = JsonConvert.DeserializeObject<CommentsDTO>(responseBody);
            deserializedResponseBody.comments.Count.Should().Be(2);
            deserializedResponseBody.comments.Should().BeEquivalentTo(new CommentsDTO.CommentDTO[]
            {
                new (
                    _comment1Response1Id,
                    _comment1Response1.Author.Id,
                    _comment1Response1.Content,
                    new Uri($"{_profilePicuteDomain}{_comment1Response1.Author.Id}"),
                    _comment1Response1.Author.UserName!,
                    false),
                new (
                    _comment1Response2Id,
                    _comment1Response2.Author.Id,
                    _comment1Response2.Content,
                    new Uri($"{_profilePicuteDomain}{_comment1Response2.Author.Id}"),
                    _comment1Response2.Author.UserName!,
                    false),
            });
        }
    }
}
