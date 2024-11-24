using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Services;

public class FileService : IFileService
{
    private readonly AzureBlobService _azureBlobService;

    public FileService(
        AzureBlobService azureBlobService)
    {
        _azureBlobService = azureBlobService;
    }

    public async Task<IResult> GetFileResult(string id)
    {
        var fileResult = await _azureBlobService.GetBlobAsync(id);
        return Results.File(fileResult.FileContent, fileResult.ContentType, fileResult.FileName);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var fileId = Guid.NewGuid().ToString();

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        await _azureBlobService.UploadBlobAsync(file.FileName, fileId, file.ContentType, memoryStream.ToArray());

        return fileId;
    }

    public async Task DeleteAsync(string id)
    {
        await _azureBlobService.DeleteBlobAsync(id);
    }
}
