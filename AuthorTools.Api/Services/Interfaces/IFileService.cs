namespace AuthorTools.Api.Services.Interfaces;

public interface IFileService
{
    Task<IResult> GetFileResult(string id);
    Task<IResult> UploadAsync(IFormFile file);
    Task<IResult> DeleteAsync(string id);
}
