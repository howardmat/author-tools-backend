using AuthorTools.Api.Models;
using AuthorTools.Data.Models;
using Azure.Core;

namespace AuthorTools.Api.Mappers;

public static class WorkspaceMapper
{
    public static Workspace ToEntity(this WorkspaceCreateRequest request, User owner)
    {
        return new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            IsDefault = request.IsDefault,
            Owner = owner
        };
    }

    public static Workspace ToEntity(this WorkspaceUpdateRequest request, string id, User owner)
    {
        return new Workspace
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            IsDefault = request.IsDefault,
            Owner = owner
        };
    }

    public static WorkspaceResponse ToResponse(this Workspace entity)
    {
        return new WorkspaceResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Icon = entity.Icon,
            IsDefault = entity.IsDefault
        };
    }
}
