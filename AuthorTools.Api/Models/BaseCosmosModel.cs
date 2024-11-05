using System.Text.Json.Serialization;

namespace AuthorTools.Api.Models;

public abstract class BaseCosmosModel
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    public string? PartitionKey { get; set; }
}