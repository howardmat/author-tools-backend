using AuthorTools.Api.Models;
using AuthorTools.Data.Models;

namespace AuthorTools.Api.Mappers;

public static class CommonEntityMapper
{
    public static T ToEntity<T>(this CommonEntityCreateRequest request, User owner) 
        where T : CommonEntity, new()
    {
        return new T
        {
            Name = request.Name,
            ImageFileId = request.ImageFileId,
            WorkspaceId = request.WorkspaceId,
            Owner = owner
        };
    }

    public static T ToEntity<T>(this CommonEntityUpdateRequest request, string id, User owner) 
        where T : CommonEntity, new()
    {
        return new T
        {
            Id = id,
            Name = request.Name,
            ImageFileId = request.ImageFileId,
            Order = request.Order,
            DetailSections = request.DetailSections,
            Owner = owner
        };
    }

    public static CommonEntityResponse ToResponse<T>(this T entity) 
        where T : CommonEntity 
    {
        return new CommonEntityResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageFileId = entity.ImageFileId,
            Order = entity.Order,
            DetailSections = entity.DetailSections
        };
    }
}
