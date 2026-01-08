namespace AuthorTools.Api.Models;

public record WorkspaceResponse
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public bool IsDefault { get; init; }
}
