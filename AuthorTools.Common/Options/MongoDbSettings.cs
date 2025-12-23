namespace AuthorTools.Common.Options;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public ContainerNames ContainerNames { get; set; } = new();
    public bool ForcePartitionKey { get; set; }
}

public class ContainerNames
{
    public string UserSettings { get; set; } = null!;
    public string Workspace { get; set; } = null!;
    public string Character { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Creature { get; set; } = null!;
}