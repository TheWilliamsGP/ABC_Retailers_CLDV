using Azure;
using Azure.Storage.Files.Shares;

public class FileService
{
    private readonly ShareServiceClient _shareServiceClient;

    public FileService(IConfiguration configuration)
    {
        _shareServiceClient = new ShareServiceClient(configuration["AzureStorage:ConnectionString"]);
    }

    public ShareClient GetShare(string shareName)
    {
        var share = _shareServiceClient.GetShareClient(shareName);
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
            await file.UploadRangeAsync(new HttpRange(0, ms.Length), ms);
        }
    }
}