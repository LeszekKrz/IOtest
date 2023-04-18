using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Jobs;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Services.VideoServices
{
    public class VideoProcessingService : BackgroundService, IVideoProcessingService
    {
        private readonly Channel<VideoProcessJob> _videoProcessingChannel = Channel.CreateUnbounded<VideoProcessJob>();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const string _mp4Extension = ".mp4";

        public VideoProcessingService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async ValueTask EnqueVideoProcessingJobAsync(VideoProcessJob videoProcessJob) => await _videoProcessingChannel.Writer.WriteAsync(videoProcessJob);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, Directory.GetCurrentDirectory());
            FFmpeg.SetExecutablesPath(Directory.GetCurrentDirectory());

            await foreach (var videoProcessJob in _videoProcessingChannel.Reader.ReadAllAsync(stoppingToken))
            {
                await ConvertToMP4AndUploadVideoAsync(videoProcessJob, stoppingToken);
            }
        }

        private async Task ConvertToMP4AndUploadVideoAsync(VideoProcessJob videoProcessJob, CancellationToken cancellationToken)
        {
            Video? video = null;
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var videoService = serviceScope.ServiceProvider.GetRequiredService<IVideoService>();
            var blobVideoService = serviceScope.ServiceProvider.GetRequiredService<IBlobVideoService>();

            try
            {
                video = await videoService.GetVideoByIdAsync(videoProcessJob.VideoId, cancellationToken);

                if (video == null) return;

                if (video.ProcessingProgress != ProcessingProgress.Uploading) return;

                if (videoProcessJob.Extension == _mp4Extension)
                {

                    await blobVideoService.UploadVideoAsync(videoProcessJob.VideoId.ToString(), videoProcessJob.VideoStream, cancellationToken);
                    await videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, cancellationToken);
                    return;
                }

                await videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Processing, cancellationToken);

                var inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{videoProcessJob.VideoId}{videoProcessJob.Extension}");
                var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{videoProcessJob.VideoId}{_mp4Extension}");

                await using var inputFileStream = File.Create(inputFilePath);
                await videoProcessJob.VideoStream.CopyToAsync(inputFileStream, cancellationToken);
                await inputFileStream.FlushAsync(cancellationToken);
                inputFileStream.Close();

                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{inputFilePath}\"")
                    .SetOutput(outputFilePath)
                    .SetOverwriteOutput(true);

                await conversion.Start(cancellationToken);

                File.Delete(inputFilePath);

                var mediaInfo = await FFmpeg.GetMediaInfo(outputFilePath);
                await videoService.SetVideoLengthAsync(video, mediaInfo.Duration.TotalSeconds, cancellationToken);

                await using var outputFileStream = File.OpenRead(outputFilePath);
                await blobVideoService.UploadVideoAsync(videoProcessJob.VideoId.ToString(), outputFileStream, cancellationToken);
                outputFileStream.Close();
                File.Delete(outputFilePath);
                await videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, cancellationToken);
            }
            catch
            {
                if (video != null)
                    await videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.FailedToUpload, cancellationToken);
            }
        }
    }
}
