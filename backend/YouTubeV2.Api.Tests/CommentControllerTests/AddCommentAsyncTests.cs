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
    public class AddCommentAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
        private readonly DateTimeOffset _utcNow = DateTimeOffset.UtcNow;
        private readonly User _user = new()
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
        private Video _video = null!;
        private string _commentAuthorId = null!;
        private Guid _videoId;


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
                Author = _user,
            };
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);
            _webApplicationFactory = Setup.GetWebApplicationFactory(_dateTimeProviderMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  await userManager.CreateAsync(_commentAuthor);
                  _commentAuthorId = await userManager.GetUserIdAsync(_commentAuthor);
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
        public async Task AddCommentAsyncWithValidContent_ShouldAddToDataBase()
        {
            // ARRANGE
            string commentContent = "Test comment content";
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _commentAuthorId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"comment?id={_videoId}", new StringContent(commentContent, Encoding.UTF8, "text/plain"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Comment? commentResult = await context
                        .Comments
                        .Include(comment => comment.Video)
                        .Include(comment => comment.Author)
                        .SingleOrDefaultAsync();

                    commentResult.Should().NotBeNull();
                    commentResult!.Content.Should().Be(commentContent);
                    commentResult.CreateDate.Should().Be(_utcNow);
                    commentResult.Author.Id.Should().Be(_commentAuthorId);
                    commentResult.Video.Id.Should().Be(_videoId);
                });
        }

        [TestMethod]
        public async Task AddCommentAsyncEmptyComment_ShouldReturnStatusCodeBadRequest()
        {
            // ARRANGE
            string commentContent = new ('a', CommentConstants.commentMaxLength + 1);
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccess(Role.Creator)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.PostAsync($"comment?id={_videoId}", new StringContent(commentContent, Encoding.UTF8, "text/plain"));

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            responseBody.Should().Be($"Comment must be at most {CommentConstants.commentMaxLength} character long");
        }
    }
}
