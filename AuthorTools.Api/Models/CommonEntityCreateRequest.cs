namespace AuthorTools.Api.Models;

public record CommonEntityCreateRequest
{
    public string? Name { get; init; }
    public string? WorkspaceId { get; init; }
    public string? ImageFileId { get; set; }
}
