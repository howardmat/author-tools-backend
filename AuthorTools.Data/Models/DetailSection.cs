namespace AuthorTools.Data.Models;

public class DetailSection
{
    public string Id { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? NoteContent { get; set; }
    public IEnumerable<Attribute> Attributes { get; set; } = [];
}
