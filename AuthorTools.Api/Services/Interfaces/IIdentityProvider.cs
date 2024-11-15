using AuthorTools.Api.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IIdentityProvider
{
    User GetCurrentUser();
    void SetCurrentUser(User user);
}
