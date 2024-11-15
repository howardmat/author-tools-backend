using AuthorTools.Api.Models;
using Azure.Storage.Blobs;

namespace AuthorTools.Api.Services;

public class FileStorageService
{
    private const string METADATA_FILENAME = "Filename";
    private const string METADATA_CONTENTTYPE = "ContentType";

    private readonly IConfiguration _configuration;
    private readonly BlobContainerClient _containerClient;

    public FileStorageService(IConfiguration configuration)
    {
        _configuration = configuration;

        //todo move these config values to appsettings
        var blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("UserFileStorageAccount"));
        _containerClient = blobServiceClient.GetBlobContainerClient("user-storage-container");
    }

    public async Task<FileStorageResult> GetBlobAsync(string fileId)
    {
        var blobClient = _containerClient.GetBlobClient(fileId);
        var result = (await blobClient.DownloadContentAsync()).Value;

        return new FileStorageResult(
            result.Content.ToArray(),
            result.Details.Metadata[METADATA_FILENAME],
            result.Details.Metadata[METADATA_CONTENTTYPE]);
    }

    public async Task UploadBlobAsync(string filename, string fileId, string contentType, byte[] fileContent)
    {
        var meta = new Dictionary<string, string>
        {
            { METADATA_FILENAME, filename },
            { METADATA_CONTENTTYPE, contentType }
        };

        var blobClient = _containerClient.GetBlobClient(fileId);
        using var memoryStream = new MemoryStream(fileContent);

        await blobClient.UploadAsync(memoryStream, true);
        await blobClient.SetMetadataAsync(meta);
    }
}
