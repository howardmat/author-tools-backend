namespace AuthorTools.Api.Models;

public record CommonEntityCreateRequest
{
    public required string Name { get; init; }
    public required string WorkspaceId { get; init; }
    public string? ImageFileId { get; set; }
}
