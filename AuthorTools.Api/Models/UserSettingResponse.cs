namespace AuthorTools.Api.Models;

public record UserSettingResponse
{
    public string? Id { get; init; }
    public string? Theme { get; init; }
}
