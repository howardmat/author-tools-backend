namespace AuthorTools.SharedLib.Models;

public class PatchRequest
{
    public string Operation { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public object? Value { get; set; }
}