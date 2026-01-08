namespace AuthorTools.Api.Models;

public record UserSettingCreateRequest
{
    public string? Theme { get; init; }
}
