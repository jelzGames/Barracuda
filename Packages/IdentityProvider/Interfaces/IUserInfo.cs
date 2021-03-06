﻿using Barracuda.Indentity.Provider.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface IUserInfo
    {
        string UserId { get; }
        string Email { get; }
        Task<string> GetTokenAsync();
        List<string> Scopes { get; }

        Result<ClaimsPrincipal> ValidateTokenAsync(HttpRequestHeaders value, IPAddress ipAddress, List<string> scopes = null, List<string> tenants = null);
        Result<bool> validScopes(List<string> scopes);
        Result<bool> validTenants(List<string> tenants);
    }
}
