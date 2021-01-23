using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthOpenId.Interfaces
{
    public interface ITokens
    {
        string CreateToken(string email, string id);
        ClaimsPrincipal ValidateToken(string authToken, bool validateLifetime = true);
    }
}
