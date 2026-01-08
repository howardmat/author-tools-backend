using AuthorTools.Data.Models;

namespace AuthorTools.Api.Models;

public class CommonEntityResponse
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ImageFileId { get; set; }
    public int? Order { get; set; }

    public IEnumerable<DetailSection> DetailSections { get; set; } = [];
}
