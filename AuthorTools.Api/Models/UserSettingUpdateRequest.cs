namespace AuthorTools.Api.Models;

public record UserSettingUpdateRequest
{
    public string? Theme { get; init; }
}
