using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Services;

public class FileService(AzureBlobService azureBlobService) : IFileService
{
    public async Task<IResult> GetFileResult(string id)
    {
        var fileResult = await azureBlobService.GetBlobAsync(id);
        return Results.File(fileResult.FileContent, fileResult.ContentType, fileResult.FileName);
    }

    public async Task<IResult> UploadAsync(IFormFile file)
    {
        var fileId = Guid.NewGuid().ToString();

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        await azureBlobService.UploadBlobAsync(file.FileName, fileId, file.ContentType, memoryStream.ToArray());

        return Results.Ok(fileId);
    }

    public async Task<IResult> DeleteAsync(string id)
    {
        await azureBlobService.DeleteBlobAsync(id);
        return Results.Ok();
    }
}
