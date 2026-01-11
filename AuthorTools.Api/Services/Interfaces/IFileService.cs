using AuthorTools.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthorTools.Api.Services.Interfaces;

public interface IFileService
{
    Task<IResult> GetFileResult(string id);
    Task<Ok<UploadFileResponse>> UploadAsync(IFormFile file);
    Task<IResult> DeleteAsync(string id);
}
