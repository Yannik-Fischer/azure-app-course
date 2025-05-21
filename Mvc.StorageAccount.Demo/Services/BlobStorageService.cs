using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Mvc.StorageAccount.Demo.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageService(IConfiguration configuration, BlobContainerClient blobContainerClient)
        {
            _configuration = configuration;
            _blobContainerClient = blobContainerClient;
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task<string> UploadBlob(IFormFile formFile, string imageName, string? originalBlobName = null)
        {
            var blobname = $"{imageName}{Path.GetExtension(formFile.FileName)}";

            if (!string.IsNullOrEmpty(originalBlobName))
            {
                await DeleteBlob(originalBlobName);
            }

            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var blobClient = _blobContainerClient.GetBlobClient(blobname);
                await blobClient.UploadAsync(content: memoryStream, overwrite: true);
            }

            return blobname;
        }

        public async Task<string> GetBlobUrl(string imageName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(imageName);

            var blobSasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(2),
                Protocol = SasProtocol.Https,
                Resource = "b"
            };
            blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            return blobClient.GenerateSasUri(blobSasBuilder).ToString();
        }

        public async Task DeleteBlob(string imageName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(imageName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        [Obsolete]
        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            var container = new BlobContainerClient(_configuration["AzureStorage:ConnectionString"], _configuration["AzureStorage:BlobContainerName"]);

            await container.CreateIfNotExistsAsync();

            return container;
        }
    }
}
