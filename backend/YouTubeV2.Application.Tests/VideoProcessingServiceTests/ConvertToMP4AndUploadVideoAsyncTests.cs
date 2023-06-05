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
using YouTubeV2.Application.FileInspector;

namespace YouTubeV2.Application.Tests.VideoProcessingServiceTests
{
    [TestClass]
    public class ConvertToMP4AndUploadVideoAsyncTests
    {
        private VideoProcessingService _videoProcessingService = null!;
        private MethodInfo _convertToMP4AndUploadVideoAsyncMethod = null!;
        private readonly Mock<IVideoService> _videoServiceMock = new();
        private readonly Mock<IBlobVideoService> _blobVideoServiceMock = new();
        private readonly Mock<IFileInspector> _fileInspectorMock = new();

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
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IFileInspector)))
                .Returns(_fileInspectorMock.Object);

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

            _fileInspectorMock
                .Setup(service => service.OpenRead(It.IsAny<string>()))
                .Returns((string filePath) => File.OpenRead(filePath));
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
            _videoServiceMock
                .Setup(service => service.SetVideoLengthAsync(video, It.IsAny<double>(), It.IsAny<CancellationToken>()))
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

            //await using var mp4FileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
            var videoProcessJob = new VideoProcessJob(videoId, videoFilePath, fileExtension);

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoLengthAsync(video, It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
            _fileInspectorMock.Verify(service => service.OpenRead(videoFilePath), Times.Once);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, It.IsAny<CancellationToken>()), Times.Once);
            _fileInspectorMock.Verify(service => service.Delete(videoFilePath), Times.Once);
        }

        [TestMethod]
        [DataRow(".avi")]
        [DataRow(".mkv")]
        [DataRow(".webm")]
        public async Task ForNotMp4FileShouldConvertAndUploadVideo(string videoFileExtension)
        {
            // ARRANGE
            var videoId = Guid.NewGuid();
            var video = new Video { Id = videoId, ProcessingProgress = ProcessingProgress.Uploading };

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
                $"video{videoFileExtension}"));

            _fileInspectorMock
                .Setup(service => service.Delete(It.IsAny<string>()))
                .Callback((string path) =>
                {
                    if (path != videoFilePath)
                        File.Delete(path);
                });

            var videoProcessJob = new VideoProcessJob(videoId, videoFilePath, videoFileExtension);
            var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}.mp4");

            // ACT
            await (Task)_convertToMP4AndUploadVideoAsyncMethod.Invoke(_videoProcessingService, new object[] { videoProcessJob, CancellationToken.None })!;

            // ASSERT
            _videoServiceMock.Verify(service => service.GetVideoByIdAsync(videoId, It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoLengthAsync(video, It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Processing, It.IsAny<CancellationToken>()), Times.Once);
            _fileInspectorMock.Verify(service => service.Delete(videoFilePath), Times.Once);
            _fileInspectorMock.Verify(service => service.OpenRead(outputFilePath), Times.Once);
            _blobVideoServiceMock.Verify(service => service.UploadVideoAsync(videoId.ToString(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            _fileInspectorMock.Verify(service => service.Delete(outputFilePath), Times.Once);
            _videoServiceMock.Verify(service => service.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, It.IsAny<CancellationToken>()), Times.Once);
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{videoId}.mp4")).Should().BeFalse();
        }
    }
}