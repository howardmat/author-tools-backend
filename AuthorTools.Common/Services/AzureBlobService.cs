using AuthorTools.Common.Models;
using Azure.Storage.Blobs;

namespace AuthorTools.Api.Services;

public class AzureBlobService
{
    private const string METADATA_FILENAME = "Filename";
    private const string METADATA_CONTENTTYPE = "ContentType";

    private readonly BlobContainerClient _containerClient;

    public AzureBlobService(
        string connectionString,
        string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<IEnumerable<string>> GetAllFileIdsAsync()
    {
        var fileIds = new List<string>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync())
        {
            fileIds.Add(blobItem.Name);
        }

        return fileIds;
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

    public async Task DeleteBlobAsync(string fileId)
    {
        var blobClient = _containerClient.GetBlobClient(fileId);
        await blobClient.DeleteAsync();
    }
}
