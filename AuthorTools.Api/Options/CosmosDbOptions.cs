namespace AuthorTools.Api.Options;

public abstract class CosmosDbOptions
{
    public string Url { get; set; } = string.Empty;
    public string PrimaryKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
