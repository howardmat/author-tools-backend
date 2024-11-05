namespace AuthorTools.Api.Options;

public abstract class CosmosDbOptions
{
    public string? Url { get; set; }
    public string? PrimaryKey { get; set; }
    public string? DatabaseName { get; set; }
    public string? ContainerName { get; set; }
}
