namespace AuthorTools.Api.Models;

public abstract class BaseCosmosModel
{
    public string? Id { get; set; }
    public string? PartitionKey { get; set; }
    public DateTimeOffset? CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
    public User? Owner { get; set; }
}