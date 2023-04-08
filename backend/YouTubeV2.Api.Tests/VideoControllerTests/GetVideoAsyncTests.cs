using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application.Services.AzureServices.BlobServices;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    public class GetVideoAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IBlobVideoService> _blobVideoService = new();
        private readonly byte[] _wholeFileStreamContent = Encoding.UTF8.GetBytes("testStreamContent");

        [TestInitialize]
        public void Initialize()
        {
            _blobVideoService
                .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(_wholeFileStreamContent));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobVideoService.Object);
        }

        [TestMethod]
        public async Task GetVideoAsyncWithRangeHeaderShouldReturnPartOfTheFile()
        {
            // ARRANGE
            int from = 2, to = 6;
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithSimpleAccess()).CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}");
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
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithSimpleAccess()).CreateClient();

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"video/{It.IsAny<Guid>()}");

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
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithSimpleAccess()).CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}");
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
            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithSimpleAccess()).CreateClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"video/{It.IsAny<Guid>()}");
            requestMessage.Headers.Range = new RangeHeaderValue(from, to);

            // ACT
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            // ASSERT
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.RequestedRangeNotSatisfiable);
        }
    }
}