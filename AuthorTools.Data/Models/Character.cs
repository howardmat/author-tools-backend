namespace AuthorTools.Data.Models;

public class Character : BaseCosmosModel
{
    public string? Name { get; set; }
    public string? ImageFileId { get; set; }
    public int? Order { get; set; }

    public IEnumerable<DetailSection> DetailSections { get; set; } = [];
}
