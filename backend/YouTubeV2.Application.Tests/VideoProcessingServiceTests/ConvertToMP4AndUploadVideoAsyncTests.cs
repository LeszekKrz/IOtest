using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using Xabe.FFmpeg.Downloader;
using Xabe.FFmpeg;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Jobs;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services.VideoServices;
using FluentAssertions;

namespace YouTubeV2.Application.Tests.VideoProcessingServiceTests
{
    [TestClass]
    public class ConvertToMP4AndUploadVideoAsyncTests
    {
        private VideoProcessingService _videoProcessingService = null!;
        private MethodInfo _convertToMP4AndUploadVideoAsyncMethod = null!;
        private readonly Mock<IVideoService> _videoServiceMock = new();
        private readonly Mock<IBlobVideoService> _blobVideoServiceMock = new();

        [TestInitialize]
        public async Task Initialize()
        {
            Mock<IServiceProvider> serviceProviderMock = new();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IVideoService)))
                .Returns(_videoServiceMock.Object);
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IBlobVideoService)))
                .Returns(_blobVideoServiceMock.Object);

            Mock<IServiceScope> serviceScopeMock = new();
            serviceScopeMock.SetupGet(scope => scope.ServiceProvider).Returns(serviceProviderMock.Object);

            Mock<IServiceScopeFactory> serviceScopeFactoryMock = new();
            serviceScopeFactoryMock
                .Setup(factory => factory.CreateScope())
                .Returns(serviceScopeMock.Object);

            _videoProcessingService = new VideoProcessingService(serviceScopeFactoryMock.Object);

            Type videoProcessingServiceType = _videoProcessingService.GetType();
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            _convertToMP4AndUploadVideoAsyncMethod = videoProcessingServiceType.GetMethod("ConvertToMP4AndUploadVideoAsync", bindingFlags)!;

            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, Directory.GetCurrentDirectory());
            FFmpeg.SetExecutablesPath(Directory.GetCurrentDirectory());
        }

        [TestMethod]
        public async Task ForMP4FileShouldUploadVideo()
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploading };
            const string fileExtension = ".mp4";

            _videoServiceMock
                .Setup(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(video);
            _videoServiceMock
                .Setup(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _blobVideoServiceMock
                .Setup(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            string videoFilePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "..",
                "..",
                "YouTubeV2.Application.Tests",
                "VideoProcessingServiceTests",
                "Videos",
                $"video{fileExtension}"));

            await using var mp4FileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
            var videoProcessJob = new VideoProcessJob(videoId, mp4FileStream, fileExtension);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, It.IsAny<CancellationToken>()), Times.Once);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ForAVIFileShouldConvertAndUploadVideo()
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploading };
            const string fileExtension = ".avi";

            _videoServiceMock
                .Setup(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(video);
            _videoServiceMock
                .Setup(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _blobVideoServiceMock
                .Setup(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            string videoFilePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "..",
                "..",
                "YouTubeV2.Application.Tests",
                "VideoProcessingServiceTests",
                "Videos",
                $"video{fileExtension}"));

            await using var aviFileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
            var videoProcessJob = new VideoProcessJob(videoId, aviFileStream, fileExtension);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Processing, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, It.IsAny<CancellationToken>()), Times.Once);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}{fileExtension}")).Should().BeFalse();
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}.mp4")).Should().BeFalse();
        }

        [TestMethod]
        public async Task ForMKVFileShouldConvertAndUploadVideo()
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploading };
            const string fileExtension = ".mkv";

            _videoServiceMock
                .Setup(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(video);
            _videoServiceMock
                .Setup(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _blobVideoServiceMock
                .Setup(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            string videoFilePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "..",
                "..",
                "YouTubeV2.Application.Tests",
                "VideoProcessingServiceTests",
                "Videos",
                $"video{fileExtension}"));

            await using var aviFileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
            var videoProcessJob = new VideoProcessJob(videoId, aviFileStream, fileExtension);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Processing, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, It.IsAny<CancellationToken>()), Times.Once);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}{fileExtension}")).Should().BeFalse();
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}.mp4")).Should().BeFalse();
        }

        [TestMethod]
        public async Task ForWEBMFileShouldConvertAndUploadVideo()
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploading };
            const string fileExtension = ".webm";

            _videoServiceMock
                .Setup(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(video);
            _videoServiceMock
                .Setup(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _blobVideoServiceMock
                .Setup(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            string videoFilePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "..",
                "..",
                "YouTubeV2.Application.Tests",
                "VideoProcessingServiceTests",
                "Videos",
                $"video{fileExtension}"));

            await using var aviFileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
            var videoProcessJob = new VideoProcessJob(videoId, aviFileStream, fileExtension);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Processing, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, It.IsAny<CancellationToken>()), Times.Once);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}{fileExtension}")).Should().BeFalse();
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}.mp4")).Should().BeFalse();
        }

        [TestMethod]
        public async Task ForNonExistingVideoFileShouldReturnEarly()
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploading };

            _videoServiceMock
                .Setup(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Video)null!);
            _videoServiceMock
                .Setup(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _blobVideoServiceMock
                .Setup(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var videoProcessJob = new VideoProcessJob(videoId, Stream.Null, string.Empty);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()), Times.Never);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task ForVideoWithWrongExtensionShouldReturnEarly()
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploaded };

            _videoServiceMock
                .Setup(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(video);
            _videoServiceMock
                .Setup(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _blobVideoServiceMock
                .Setup(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var videoProcessJob = new VideoProcessJob(videoId, Stream.Null, string.Empty);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, It.IsAny<ProcessingProgress>(), It.IsAny<CancellationToken>()), Times.Never);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
