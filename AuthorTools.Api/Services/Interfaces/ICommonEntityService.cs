using AuthorTools.Api.Models;
using AuthorTools.Common.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface ICommonEntityService<T>
{
    Task<IEnumerable<CommonEntityResponse>> GetAllAsync(string workspaceId);
    Task<CommonEntityResponse> GetAsync(string id);
    Task<CommonEntityResponse> CreateAsync(CommonEntityCreateRequest request);
    Task<CommonEntityResponse> UpdateAsync(string id, CommonEntityUpdateRequest request);
    Task PatchAsync(string id, IEnumerable<PatchRequest> patchRequests);
    Task DeleteAsync(string id);
}
