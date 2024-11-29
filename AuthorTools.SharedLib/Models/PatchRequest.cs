using AuthorTools.SharedLib.Common;
using Microsoft.Azure.Cosmos;

namespace AuthorTools.SharedLib.Models;

public class PatchRequest
{
    public string Operation { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public object? Value { get; set; }

    public PatchOperation ToPatchOperation()
    {
        if (int.TryParse(Value?.ToString(), out var intValue))
        {
            return BuildPatchOperation(intValue);
        }

        throw new NotSupportedException($"Type of Value is not supported");
    }

    private PatchOperation BuildPatchOperation<T>(T value)
    {
        return Operation switch
        {
            Constants.PatchOperations.UPDATE => PatchOperation.Set(Path, value),
            _ => throw new NotSupportedException($"Operation [{Operation}] is not supported"),
        };
    }
}