using System;
using System.Collections.Generic;
using System.Text;

namespace AuthOpenId.Interfaces
{
    public interface ISettingsTokens
    {
        string IUsser { get; }
        string Audencie { get; }
        string JWTKey { get; }
        int ExpiredTimeInSeconds { get; }
        string CookieToken { get; }
        string GoogleClientId { get; }
        string GoogleISSUER { get; }
        string GraphFacebook { get; }
        string GraphMicrosoft { get; }


    }
}
