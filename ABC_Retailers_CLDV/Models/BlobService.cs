using Azure.Storage.Blobs;

namespace ABC_Retailers_CLDV.Models
{
    public class BlobService
    {
        private readonly BlobServiceClient _client;

        public BlobService(IConfiguration config)
        {
            _client = new BlobServiceClient(config["AzureStorage:ConnectionString"]);
        }

        public BlobContainerClient GetContainer(string containerName)
        {
            var container = _client.GetBlobContainerClient(containerName);
            container.CreateIfNotExists();
            return container;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var container = GetContainer(containerName);
            var blob = container.GetBlobClient(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }
            return blob.Uri.ToString();
        }
    }
}
