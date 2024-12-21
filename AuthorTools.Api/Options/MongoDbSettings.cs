namespace AuthorTools.Api.Options;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public ContainerNames ContainerNames { get; set; } = new();
}

public class ContainerNames
{
    public string Character { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Creature { get; set; } = null!;
}