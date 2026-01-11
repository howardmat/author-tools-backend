using AuthorTools.Data.Models;

namespace AuthorTools.Api.Models;

public record CommonEntityResponse
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public string? ImageFileId { get; init; }
    public string? WorkspaceId { get; init; }
    public int? Order { get; init; }

    public IEnumerable<DetailSection> DetailSections { get; init; } = [];
}
