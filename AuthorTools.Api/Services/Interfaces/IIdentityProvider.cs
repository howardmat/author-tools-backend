using AuthorTools.Data.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IIdentityProvider
{
    User GetCurrentUser();
    void SetCurrentUser(User user);
}
