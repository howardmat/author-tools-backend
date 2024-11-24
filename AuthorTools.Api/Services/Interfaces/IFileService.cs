namespace AuthorTools.Api.Services.Interfaces;

public interface IFileService
{
    Task<IResult> GetFileResult(string id);
    Task<string> UploadAsync(IFormFile file);
    Task DeleteAsync(string id);
}
