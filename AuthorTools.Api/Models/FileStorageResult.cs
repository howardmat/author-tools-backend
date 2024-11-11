namespace AuthorTools.Api.Models;

public class FileStorageResult(byte[] content, string fileName, string contentType)
{
    public byte[] FileContent { get; set; } = content;
    public string FileName { get; set; } = fileName;
    public string ContentType { get; set; } = contentType;
}
