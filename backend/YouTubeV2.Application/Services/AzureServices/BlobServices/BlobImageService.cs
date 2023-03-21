using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace YouTubeV2.Application.Services.AzureServices.BlobServices
{
    public class BlobImageService : IBlobImageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobImageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public Uri GetImageUrl(string fileName, string blobContainerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            return blobContainerClient.GetBlobClient(fileName).Uri;
        }

        public async Task UploadImageAsync(byte[] bytes, string fileName, string blobContainerName, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient =  blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(
                new BinaryData(bytes),
                new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/png" } },
                cancellationToken);
        }
    }
}
