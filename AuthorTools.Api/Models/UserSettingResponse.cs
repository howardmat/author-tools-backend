namespace AuthorTools.Api.Models;

public record UserSettingResponse
{
    public required string Id { get; init; }
    public required string Theme { get; init; }
}
