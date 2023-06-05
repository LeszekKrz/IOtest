namespace YouTubeV2.Application.FileInspector
{
    public interface IFileInspector
    {
        Task CreateFileAsync(string path, Stream stream, CancellationToken cancellationToken = default);

        FileStream OpenRead(string path);

        void Delete(string path);
    }
}