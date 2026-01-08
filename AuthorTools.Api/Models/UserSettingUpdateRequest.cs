namespace AuthorTools.Api.Models;

public record UserSettingUpdateRequest
{
    public required string Theme { get; init; }
}
