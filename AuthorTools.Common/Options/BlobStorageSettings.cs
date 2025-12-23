namespace AuthorTools.Common.Options;

public class BlobStorageSettings
{
    public string ConnectionString { get; set; } = null!;
    public string ContainerName { get; set; } = null!;
}
