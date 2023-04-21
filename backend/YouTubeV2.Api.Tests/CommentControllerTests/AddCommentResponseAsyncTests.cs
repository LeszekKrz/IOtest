using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Text;
using YouTubeV2.Api.Enums;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Providers;

namespace YouTubeV2.Api.Tests.CommentControllerTests
{
    [TestClass]
    public class AddCommentResponseAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
        private readonly DateTimeOffset _utcNow = DateTimeOffset.UtcNow;
        private readonly User _videoAuthor = new()
        {
            Email = "test@mail.com",
            UserName = "testUsername",
            Name = "testName",
            Surname = "testSurname",
        };
        private readonly User _commentAuthor = new()
        {
            Email = "comment@mail.com",
            UserName = "commentUsername",
            Name = "commentName",
            Surname = "commentSurname",
        };
        private readonly User _responseAuthor = new()
        {
            Email = "response@mail.com",
            UserName = "responseUsername",
            Name = "responseName",
            Surname = "responseSurname",
        };

        private Video _video = null!;
        private string _commentAuthorId = null!;
        private string _responseAuthorId = null!;
        private Guid _commentId;


        [TestInitialize]
        public async Task Initialize()
        {
            _video = new()
            {
                Title = "test title",
                Description = "test description",
                Visibility = Visibility.Public,
                UploadDate = DateTimeOffset.UtcNow,
                EditDate = DateTimeOffset.UtcNow,
                Author = _videoAuthor,
                Comments = new List<Comment>()
                {
                    new()
                    {
                        Content = "other comment1 content",
                        CreateDate = DateTimeOffset.UtcNow,
                        Author = _commentAuthor,
                    }
                }
            };
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);
            _webApplicationFactory = Setup.GetWebApplicationFactory(_dateTimeProviderMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_videoAuthor);
                  await userManager.CreateAsync(_commentAuthor);
                  await userManager.CreateAsync(_responseAuthor);
                  _commentAuthorId = await userManager.GetUserIdAsync(_commentAuthor);
                  _responseAuthorId = await userManager.GetUserIdAsync(_responseAuthor);
              });

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_videoAuthor);
                    context.Users.Attach(_commentAuthor);
                    var video = await context.Videos.AddAsync(_video);
                    await context.SaveChangesAsync();
                    _commentId = video.Entity.Comments.Single().Id;
                });
        }

        [TestMethod]
        public async Task AddCommentResponseAsyncWithValidContent_ShouldAddToDataBase()
        {
            // ARRANGE
            string responseCommentContent = "Test comment response content";
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _responseAuthorId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"comment/response?id={_commentId}", new StringContent(responseCommentContent, Encoding.UTF8, "text/plain"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    CommentResponse? commentResponseResult = await context
                        .CommentResponses
                        .Include(commentResponse => commentResponse.Author)
                        .Include(commentResponse => commentResponse.RespondOn)
                        .SingleOrDefaultAsync(commentResponse => commentResponse.RespondOn.Id == _commentId);

                    commentResponseResult.Should().NotBeNull();
                    commentResponseResult!.Content.Should().Be(responseCommentContent);
                    commentResponseResult.CreateDate.Should().Be(_utcNow);
                    commentResponseResult.Author.Id.Should().Be(_responseAuthorId);
                    commentResponseResult.RespondOn.Id.Should().Be(_commentId);
                });
        }

        [TestMethod]
        public async Task AddCommentResponseAsyncEmptyComment_ShouldReturnStatusCodeBadRequest()
        {
            // ARRANGE
            string responseCommentContent = new('a', CommentConstants.commentMaxLength + 1);
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccess(Role.Creator)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"comment/response?id={_commentId}", new StringContent(responseCommentContent, Encoding.UTF8, "text/plain"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            responseBody.Should().Be($"Comment response must be at most {CommentConstants.commentMaxLength} character long");
        }
    }
}
