using Azure;
using Azure.Storage.Blobs;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text;
using YouTubeV2.Application.Configurations.BlobStorage;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Tests.BlobVideoServiceTests
{
    [TestClass]
    public class GetVideoAsyncTests
    {
        private Mock<BlobServiceClient> _blobServiceClientMock = new Mock<BlobServiceClient>();
        private Mock<BlobContainerClient> _blobContainerClientMock = new Mock<BlobContainerClient>();
        private Mock<BlobClient> _blobClientMock = new Mock<BlobClient>();
        private BlobVideoService _blobVideoService = null!;
        private byte[] _expectedStreamContent = Encoding.UTF8.GetBytes("testStreamContent");


        [TestInitialize]
        public void Initialize()
        {
            _blobClientMock
                .Setup(x => x.OpenReadAsync(It.IsAny<bool>(), It.IsAny<long>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(_expectedStreamContent));
            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);
            _blobServiceClientMock
                .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_blobContainerClientMock.Object);
            _blobVideoService = new BlobVideoService(
                _blobServiceClientMock.Object,
                Options.Create(new BlobStorageVideosConfig { VideosContainerName = It.IsAny<string>() }));
        }

        [TestMethod]
        public async Task GetVideoAsyncShouldReturnFullReadOnlyVideoStream()
        {
            // ARRANGE
            _blobClientMock
                .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

            // ACT
            Stream stream = await _blobVideoService.GetVideoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>());

            // ASSERT
            stream.Length.Should().Be(_expectedStreamContent.Length);
            var streamContent = new byte[_expectedStreamContent.Length];
            await stream.ReadAsync(streamContent, 0, _expectedStreamContent.Length);
            for (int i = 0; i < _expectedStreamContent.Length; i++)
                streamContent[i].Should().Be(_expectedStreamContent[i]);
        }

        [TestMethod]
        public void GetVideoThatDoesntExistShouldThrowAnException()
        {
            // ARRANGE
            _blobClientMock
                .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(false, new Mock<Response>().Object));

            // ACT
            Func<Task> action = async () => await _blobVideoService.GetVideoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>());

            // ASSERT
            action.Should().ThrowAsync<FileNotFoundException>().WithMessage($"There is no video with fileName {It.IsAny<string>()}");
        }
    }
}
