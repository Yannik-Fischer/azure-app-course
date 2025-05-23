﻿
namespace Mvc.StorageAccount.Demo.Services
{
    public interface IBlobStorageService
    {
        Task<string> GetBlobUrl(string imageName);
        Task DeleteBlob(string imageName);
        Task<string> UploadBlob(IFormFile formFile, string imageName, string? originalBlobName = null);
    }
}