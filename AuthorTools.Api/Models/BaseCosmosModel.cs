namespace AuthorTools.Api.Models;

public abstract class BaseCosmosModel
{
    public string? Id { get; set; }
    public string? PartitionKey { get; set; }
}