namespace AuthorTools.Api.Models;

public record WorkspaceCreateRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public bool IsDefault { get; init; }
}
