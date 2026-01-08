namespace AuthorTools.Api.Models;

public record UserSettingCreateRequest
{
    public required string Theme { get; init; }
}
