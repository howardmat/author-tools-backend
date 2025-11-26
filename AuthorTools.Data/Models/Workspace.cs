namespace AuthorTools.Data.Models;

public class Workspace : BaseMongoModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsDefault { get; set; }
}
