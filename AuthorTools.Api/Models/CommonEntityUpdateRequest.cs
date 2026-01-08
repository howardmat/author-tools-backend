using AuthorTools.Data.Models;

namespace AuthorTools.Api.Models;

public record CommonEntityUpdateRequest
{
    public string? Name { get; init; }
    public string? ImageFileId { get; set; }
    public int? Order { get; set; }

    public IEnumerable<DetailSection> DetailSections { get; set; } = [];
}
