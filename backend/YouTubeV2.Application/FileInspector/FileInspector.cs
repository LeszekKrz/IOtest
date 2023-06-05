namespace YouTubeV2.Application.FileInspector
{
    public class FileInspector : IFileInspector
    {
        public async Task CreateFileAsync(string path, Stream stream, CancellationToken cancellationToken = default)
        {
            await using var fileStream = File.Create(path);
            await stream.CopyToAsync(fileStream, cancellationToken);
            await fileStream.FlushAsync(cancellationToken);
        }

        public void Delete(string path) => File.Delete(path);

        public FileStream OpenRead(string path) => File.OpenRead(path);
    }
}