using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.FileInspector;
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
            var fileInspector = serviceScope.ServiceProvider.GetRequiredService<IFileInspector>();

            try
            {
                video = await videoService.GetVideoByIdAsync(videoProcessJob.VideoId, cancellationToken);

                if (video == null) return;

                if (video.ProcessingProgress != ProcessingProgress.Uploading) return;

                var mediaInfo = await FFmpeg.GetMediaInfo(videoProcessJob.Path, cancellationToken);
                await videoService.SetVideoLengthAsync(video, mediaInfo.Duration.TotalSeconds, cancellationToken);

                if (videoProcessJob.Extension == _mp4Extension)
                {
                    await using (FileStream videoStream = fileInspector.OpenRead(videoProcessJob.Path))
                    {
                        await blobVideoService.UploadVideoAsync(videoProcessJob.VideoId.ToString(), videoStream, cancellationToken);
                    }
                    
                    await videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Ready, cancellationToken);
                    fileInspector.Delete(videoProcessJob.Path);
                    return;
                }

                await videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Processing, cancellationToken);

                var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{videoProcessJob.VideoId}{_mp4Extension}");

                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{videoProcessJob.Path}\"")
                    .SetOutput(outputFilePath)
                    .SetOverwriteOutput(true);

                await conversion.Start(cancellationToken);

                fileInspector.Delete(videoProcessJob.Path);

                await using (FileStream outputFileStream = fileInspector.OpenRead(outputFilePath))
                {
                    await blobVideoService.UploadVideoAsync(videoProcessJob.VideoId.ToString(), outputFileStream, cancellationToken);
                }
                
                fileInspector.Delete(outputFilePath);

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
