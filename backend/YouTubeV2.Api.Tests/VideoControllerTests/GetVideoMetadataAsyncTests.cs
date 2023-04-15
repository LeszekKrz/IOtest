using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application;
using Microsoft.AspNetCore.Identity;
using YouTubeV2.Api.Enums;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using YouTubeV2.Application.DTO;
using System.Web;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class GetVideoMetadataAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IUserService> _userService = new();

        [TestInitialize]
        public async Task Initialize()
        {
            _userService
                .Setup(x => x.ValidateToken(It.IsAny<string>()))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.Role, Role.Simple) })));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_userService.Object);

            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task TestMethod1Async()
        {
            // ARRANGE
            var httpClient = _webApplicationFactory.CreateClient();

            User user = await _webApplicationFactory.DoWithinScopeWithReturn<UserManager<User>, User>(
                async manager =>
                {
                    User user = new("creator@mail.com", "nick", "Jan", "Kowlaski");
                    await manager.CreateAsync(user);
                    return user;
                });

            var video = new Video("Title", "description", Visibility.Public, new string[]{"tag1", "tag2"}, user, DateTimeOffset.UtcNow);

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(user);
                    await context.Videos.AddAsync(video);
                    await context.SaveChangesAsync();
                });

            // ACT
            var response = await httpClient.GetAsync($"video-metadata?id={video.Id}");

            var content = await response.Content.ReadAsStringAsync();
            var videoResponseDto = JsonConvert.DeserializeObject<VideoMetadataDto>(content);

            // ASSERT
            response.Content.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            videoResponseDto.title.Should().Be(video.Title);
            videoResponseDto.description.Should().Be(video.Description);
            videoResponseDto.visibility.Should().Be(video.Visibility.ToString());
            videoResponseDto.tags.Should().BeEquivalentTo(video.Tags.Select(tag => tag.ToString()));
            videoResponseDto.uploadDate.Should().Be(video.UploadDate.DateTime);
            videoResponseDto.viewCount.Should().Be(0);
        }
    }
}
