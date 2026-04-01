using Azure.Storage.Files.Shares;

namespace ABC_Retailers_CLDV.Models
{
    public class FileService
    {
        private readonly ShareServiceClient _client;

        public FileService(IConfiguration config)
        {
            _client = new ShareServiceClient(config["AzureStorage:ConnectionString"]);
        }

        public ShareClient GetShare(string shareName)
        {
            var share = _client.GetShareClient(shareName);
            share.CreateIfNotExists();
            return share;
        }

        public async Task UploadLogAsync(string shareName, string fileName, string content)
        {
            var share = GetShare(shareName);
            var root = share.GetRootDirectoryClient();
            var file = root.GetFileClient(fileName);
            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                await file.CreateAsync(ms.Length);
                await file.UploadRangeAsync(new Azure.HttpRange(0, ms.Length), ms);
            }
        }
    }
}
