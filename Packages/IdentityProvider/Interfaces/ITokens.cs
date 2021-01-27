using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ITokens
    {
        string CreateToken(string id, string email, dynamic userScopes, bool iSforgotPasswordOrRegister = false);
        ClaimsPrincipal ValidateToken(string authToken, bool validateLifetime = true);
    }
}
