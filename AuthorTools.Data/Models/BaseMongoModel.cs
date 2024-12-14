using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthorTools.Data.Models;

public abstract class BaseMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? PartitionKey { get; set; }
    public DateTimeOffset? CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
    public User? Owner { get; set; }

    public object? this[string propertyName]
    {
        get { return this.GetType()?.GetProperty(propertyName)?.GetValue(this, null); }
        set { this.GetType()?.GetProperty(propertyName)?.SetValue(this, value, null); }
    }
}