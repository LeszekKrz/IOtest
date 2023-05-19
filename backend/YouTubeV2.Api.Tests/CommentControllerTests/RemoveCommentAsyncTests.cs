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
    public class RemoveCommentAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private User _commentOwner = null!;
        private readonly User _notCommentOwner = new ()
        {
            Email = "notOwner@mail.com",
            UserName = "notOwnerUsername",
            Name = "notOwnerName",
            Surname = "notOwnerSurname",
        };
        private Video _video = null!;
        private Comment _comment1 = null!;
        private Comment _comment2 = null!;
        private string _commentOwnerId = null!;
        private string _notCommentOwnerId = null!;
        private Guid _comment1Id;
        private Guid _comment2Id;
        private Guid _comment2ResponseId;

        [TestInitialize]
        public async Task Initialize()
        {
            _commentOwner = new()
            {
                Email = "owner@mail.com",
                UserName = "ownerUsername",
                Name = "ownerName",
                Surname = "ownerSurname",
            };

            _comment1 = new()
            {
                Content = "comment1 content",
                CreateDate = DateTimeOffset.UtcNow,
                Author = _commentOwner,
                Responses = new List<CommentResponse>()
                {
                    new CommentResponse()
                    {
                        Content = "comment1 response1",
                        CreateDate = DateTimeOffset.UtcNow,
                        Author = _commentOwner,
                    },
                    new CommentResponse()
                    {
                        Content = "comment1 response2",
                        CreateDate = DateTimeOffset.UtcNow,
                        Author = _commentOwner,
                    },
                },
            };

            _comment2 = new()
            {
                Content = "comment2 content",
                CreateDate = DateTimeOffset.UtcNow,
                Author = _commentOwner,
                Responses = new List<CommentResponse>()
                {
                    new CommentResponse()
                    {
                        Content = "comment2 response",
                        CreateDate = DateTimeOffset.UtcNow,
                        Author = _commentOwner,
                    },
                },
            };


            _video = new()
            {
                Title = "test title",
                Description = "test description",
                Visibility = Visibility.Public,
                UploadDate = DateTimeOffset.UtcNow,
                EditDate = DateTimeOffset.UtcNow,
                Author = _commentOwner,
                Comments = new[]
                {
                    _comment1,
                    _comment2
                }
            };

            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_commentOwner);
                  await userManager.CreateAsync(_notCommentOwner);
                  _commentOwnerId = await userManager.GetUserIdAsync(_commentOwner);
                  _notCommentOwnerId = await userManager.GetUserIdAsync(_notCommentOwner);
              });

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(_commentOwner);
                    var video = await context.Videos.AddAsync(_video);
                    await context.SaveChangesAsync();
                    _comment1Id = video.Entity.Comments.Single(comment => comment.Content == _comment1.Content).Id;
                    Comment comment2 = video.Entity.Comments.Single(comment => comment.Content == _comment2.Content);
                    _comment2Id = comment2.Id;
                    _comment2ResponseId = comment2.Responses.Single().Id;
                });
        }

        [TestMethod]
        public async Task RemoveCommentAsync_WhenYouAreTheOwner_ShouldRemoveCommentAndItsResponsesFromDataBase()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Creator, _commentOwnerId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"comment?id={_comment1Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Comment? deletedComment = await context.Comments.FindAsync(_comment1Id);
                    deletedComment.Should().BeNull();

                    var deletedCommentResponses = await context.CommentResponses.Where(commentResponse => commentResponse.RespondOn.Id == _comment1Id).ToListAsync();
                    deletedCommentResponses.Should().BeEmpty();

                    Comment? notDeletedComment = await context.Comments.Include(comment => comment.Responses).FirstOrDefaultAsync(comment => comment.Id == _comment2Id);
                    notDeletedComment.Should().NotBeNull();
                    notDeletedComment!.Responses.Should().HaveCount(1);
                    notDeletedComment.Responses.Single().Id.Should().Be(_comment2ResponseId);
                });
        }

        [TestMethod]
        public async Task RemoveCommentAsync_WhenYouAreNotTheOwnerButAdministrator_ShouldRemoveCommentAndItsResponsesFromDataBase()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Administrator, _notCommentOwnerId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"comment?id={_comment1Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    Comment? deletedComment = await context.Comments.FindAsync(_comment1Id);
                    deletedComment.Should().BeNull();

                    var deletedCommentResponses = await context.CommentResponses.Where(commentResponse => commentResponse.RespondOn.Id == _comment1Id).ToListAsync();
                    deletedCommentResponses.Should().BeEmpty();

                    Comment? notDeletedComment = await context.Comments.Include(comment => comment.Responses).FirstOrDefaultAsync(comment => comment.Id == _comment2Id);
                    notDeletedComment.Should().NotBeNull();
                    notDeletedComment!.Responses.Should().HaveCount(1);
                    notDeletedComment.Responses.Single().Id.Should().Be(_comment2ResponseId);
                });
        }

        [TestMethod]
        public async Task RemoveCommentAsync_WhenYouAreNotTheOwnerNeitherAdministrator_ShouldReturnForbidden()
        {
            // ARRANGE
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, _notCommentOwnerId)).CreateClient();

            // ACT
            var httpResponseMessage = await httpClient.DeleteAsync($"comment?id={_comment1Id}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
