namespace AuthorTools.Api.Models;

public record WorkspaceResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public bool IsDefault { get; init; }
}
