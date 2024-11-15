﻿using AuthorTools.Api.Exceptions;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Services;

public class UserProvider : IIdentityProvider
{
    private User? _currentUser;

    public User GetCurrentUser() => _currentUser
        ?? throw new ApplicationAuthenticationException(
            "SetCurrentUser needs to be called before GetCurrentUser");
    public void SetCurrentUser(User user) => _currentUser = user;
}