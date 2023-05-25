using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using YouTubeV2.Api.Enums;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.CommentControllerTests
{
    [TestClass]
    public class RemoveCommentResponseAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly User _otherUser = new()
        {
            Email = "notOwner@mail.com",
            UserName = "notOwnerUsername",
            Name = "notOwnerName",
            Surname = "notOwnerSurname",
        };
        private readonly User _commentsOwner = new()
        {
            Email = "owner@mail.com",
            UserName = "ownerUsername",
            Name = "ownerName",
            Surname = "ownerSurname",
        };
        private Video _video = null!;
        private Comment _comment = null!;
        private Guid _commentId;
        private CommentResponse _commentResponse1 = null!;
        private CommentResponse _commentResponse2 = null!;
        private Guid _commentResponse1Id;


        [TestInitialize]
        public async Task Initialize()
        {
            _commentResponse1 = new CommentResponse()
            {
                Content = "comment response1",
                CreateDate = DateTimeOffset.UtcNow,
                Author = _commentsOwner,
            };

            _commentResponse2 = new CommentResponse()
            {
                Content = "comment response2",
                CreateDate = DateTimeOffset.UtcNow,
                Author = _commentsOwner,
            };

            _comment = new()
            {
                Content = "comment content",
                CreateDate = DateTimeOffset.UtcNow,
                Author = _otherUser,
                Responses = new List<CommentResponse>()
                {
                    _commentResponse1,
                    _commentResponse2,
                },
            };

            _video = new()
            {
                Title = "test title",
                Description = "test description",
                Visibility = Visibility.Public,
                UploadDate = DateTimeOffset.UtcNow,
                EditDate = DateTimeOffset.UtcNow,
                Author = _otherUser,
                Comments = new[]
                {
                    _comment,
                }
            };

            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_otherUser);
                  _otherUser.Id = await userManager.GetUserIdAsync(_otherUser);
                  await userManager.CreateAsync(_commentsOwner);
                  _commentsOwner.Id = await userManager.GetUserIdAsync(_commentsOwner);
              });

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_otherUser);
                    context.Users.Attach(_commentsOwner);
                    var video = await context.Videos.AddAsync(_video);
                    await context.SaveChangesAsync();
                    var comment = video.Entity.Comments.Single();
                    _commentId = comment.Id;
                    _commentResponse1Id = comment.Responses.Single(response => response.Content == _commentResponse1.Content).Id;
                });
        }

        [TestMethod]
        public async Task RemoveCommentAsync_WhenYouAreTheOwner_ShouldRemoveCommentAndItsResponsesFromDataBase()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _commentsOwner.Id)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"api/comment/response?id={_commentResponse1Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Comment? comment = await context
                        .Comments
                        .Include(comment => comment.Responses)
                        .ThenInclude(response => response.Author)
                        .FirstOrDefaultAsync(comment => comment.Id == _commentId);

                    comment.Should().NotBeNull();
                    comment!.Responses.Should().HaveCount(1);
                    CommentResponse commentResponse = comment.Responses.Single();
                    commentResponse.Content.Should().Be(_commentResponse2.Content);
                    commentResponse.CreateDate.Should().Be(_commentResponse2.CreateDate);
                    commentResponse.Author.Id.Should().Be(_commentsOwner.Id);
                    commentResponse.RespondOn.Id.Should().Be(_commentId);
                });
        }

        [TestMethod]
        public async Task RemoveCommentAsync_WhenYouAreNotTheOwnerButAdministrator_ShouldRemoveCommentAndItsResponsesFromDataBase()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Administrator, _otherUser.Id)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"api/comment/response?id={_commentResponse1.Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Comment? comment = await context
                        .Comments
                        .Include(comment => comment.Responses)
                        .ThenInclude(response => response.Author)
                        .FirstOrDefaultAsync(comment => comment.Id == _commentId);

                    comment.Should().NotBeNull();
                    comment!.Responses.Should().HaveCount(1);
                    CommentResponse commentResponse = comment.Responses.Single();
                    commentResponse.Content.Should().Be(_commentResponse2.Content);
                    commentResponse.CreateDate.Should().Be(_commentResponse2.CreateDate);
                    commentResponse.Author.Id.Should().Be(_commentsOwner.Id);
                    commentResponse.RespondOn.Id.Should().Be(_commentId);
                });
        }

        [TestMethod]
        public async Task RemoveCommentAsync_WhenYouAreNotTheOwnerNeitherAdministrator_ShouldReturnForbidden()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, _otherUser.Id)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"api/comment/response?id={_commentResponse1Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
