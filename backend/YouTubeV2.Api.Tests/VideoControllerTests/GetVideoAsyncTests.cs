using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class GetVideoAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IBlobVideoService> _blobVideoService = new();
        private readonly Mock<IUserService> _userService = new();
        private readonly byte[] _wholeFileStreamContent = Encoding.UTF8.GetBytes("testStreamContent");

        [TestInitialize]
        public void Initialize()
        {
            _blobVideoService
                .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(_wholeFileStreamContent));
            _userService
                .Setup(x => x.ValidateToken(It.IsAny<string>()))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.Role, Role.Simple) })));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobVideoService.Object, _userService.Object);
        }

        [TestMethod]
        public async Task GetVideoAsyncWithRangeHeaderShouldReturnPartOfTheFile()
        {
            // ARRANGE
            int from = 2, to = 6;
            string testToken = "testToken";
            using HttpClient httpClient = _webApplicationFactory.CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}?access_token={testToken}");
            requestMessage.Headers.Range = new RangeHeaderValue(from, to);

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.PartialContent);
            byte[] streamContent = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            streamContent.Should().BeEquivalentTo(_wholeFileStreamContent[from..(to + 1)], options => options.WithStrictOrdering());
        }

        [TestMethod]
        public async Task GetVideoAsyncWithoutRangeHeaderShouldReturnTheWholeFile()
        {
            // ARRANGE
            string testToken = "testToken";
            using HttpClient httpClient = _webApplicationFactory.CreateClient();

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"video/{It.IsAny<Guid>()}?access_token={testToken}");

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            byte[] streamContent = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            streamContent.Should().BeEquivalentTo(_wholeFileStreamContent, options => options.WithStrictOrdering());
        }

        [TestMethod]
        public async Task GetVideoAsyncWithRangeHeaderWithEndingExtendingVideoSizeShouldReturnFileContentFromToTheEnd()
        {
            // ARRANGE
            int from = 10, to = _wholeFileStreamContent.Length;
            string testToken = "testToken";
            using HttpClient httpClient = _webApplicationFactory.CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}?access_token={testToken}");
            requestMessage.Headers.Range = new RangeHeaderValue(from, to);

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.PartialContent);
            byte[] streamContent = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            streamContent.Should().BeEquivalentTo(_wholeFileStreamContent[from..], options => options.WithStrictOrdering());
        }

        [TestMethod]
        public async Task GetVideoAsyncWithRangeHeaderWithStartExtendingVideoSizeShouldReturnStatusCodeRangeNotSatisfiable()
        {
            // ARRANGE
            int from = _wholeFileStreamContent.Length, to = _wholeFileStreamContent.Length + 1;
            string testToken = "testToken";
            using HttpClient httpClient = _webApplicationFactory.CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}?access_token={testToken}");
            requestMessage.Headers.Range = new RangeHeaderValue(from, to);

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.RequestedRangeNotSatisfiable);
        }

        [TestMethod]
        public async Task GetVideoAsyncWithInvalidAccessTokenShouldReturnStatusCodeUnauthorized()
        {
            // ARRANGE
            _userService.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns((ClaimsPrincipal?)null);

            int from = _wholeFileStreamContent.Length, to = _wholeFileStreamContent.Length + 1;
            string testToken = "testToken";
            using HttpClient httpClient = _webApplicationFactory.CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}?access_token={testToken}");
            requestMessage.Headers.Range = new RangeHeaderValue(from, to);

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task GetVideoAsyncWithAccessTokenWithWrongRoleShouldReturnStatusCodeForbidden()
        {
            // ARRANGE
            _userService
                .Setup(x => x.ValidateToken(It.IsAny<string>()))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.Role, "NON EXISTING ROLE") })));

            int from = _wholeFileStreamContent.Length, to = _wholeFileStreamContent.Length + 1;
            string testToken = "testToken";
            using HttpClient httpClient = _webApplicationFactory.CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}?access_token={testToken}");
            requestMessage.Headers.Range = new RangeHeaderValue(from, to);

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}