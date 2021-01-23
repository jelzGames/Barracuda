using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ISettingsUserSecrests
    {
        string SecretKey { get; }
        string CookieTokenPath { get; }
        string CookieRefreshToken { get; }
        string CookieRefreshTokenPath { get; }
        public bool RedisCacheSecurity { get; }
    }
}
