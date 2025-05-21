using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Mvc.StorageAccount.Demo.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private const string containerName = "attendeeimages";
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadBlob(IFormFile formFile, string imageName, string? originalBlobName = null)
        {
            var blobname = $"{imageName}{Path.GetExtension(formFile.FileName)}";
            var blobClient = await GetBlobContainerClient();

            if (!string.IsNullOrEmpty(originalBlobName))
            {
                await DeleteBlob(originalBlobName);
            }

            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var blob = blobClient.GetBlobClient(blobname);
                await blob.UploadAsync(content: memoryStream, overwrite: true);
            }

            return blobname;
        }

        public async Task<string> GetBlobUrl(string imageName)
        {
            var blobClient = await GetBlobContainerClient();
            var blob = blobClient.GetBlobClient(imageName);

            var blobSasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(2),
                Protocol = SasProtocol.Https,
                Resource = "b"
            };
            blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            return blob.GenerateSasUri(blobSasBuilder).ToString();
        }

        public async Task DeleteBlob(string imageName)
        {
            var blobClient = await GetBlobContainerClient();
            var blob = blobClient.GetBlobClient(imageName);

            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            var container = new BlobContainerClient(_configuration["StorageConnectionString"], containerName);

            await container.CreateIfNotExistsAsync();

            return container;
        }
    }
}
