using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;
using System.Security.Claims;

namespace AuthorTools.Api.Filters;

public class JwtUserEndpointFilter(IIdentityProvider identityProvider) : IEndpointFilter
{
    private readonly IIdentityProvider _userProvider = identityProvider;

    public virtual async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var identity = context.HttpContext.User.Identity;
        if (identity != null && identity.IsAuthenticated)
        {
            var id = context.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var email = context.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var firstname = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "fname")?.Value;
            var lastname = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "lname")?.Value;

            var user = new User(id, email, firstname, lastname);

            _userProvider.SetCurrentUser(user);
        }

        var result = await next(context);
        return result;
    }
}
