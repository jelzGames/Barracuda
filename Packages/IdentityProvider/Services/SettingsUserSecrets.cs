using Barracuda.Indentity.Provider.Interfaces;
using System;

namespace Barracuda.Indentity.Provider.Services
{
    public class SettingsUserSecrets: ISettingsUserSecrests
    {
        public SettingsUserSecrets()
        {
            SecretKey = System.Environment.GetEnvironmentVariable("SecretKey", EnvironmentVariableTarget.Process);
            CookieTokenPath = System.Environment.GetEnvironmentVariable("CookieTokenPath", EnvironmentVariableTarget.Process);
            CookieRefreshToken = System.Environment.GetEnvironmentVariable("CookieRefreshToken", EnvironmentVariableTarget.Process);
            CookieRefreshTokenPath = System.Environment.GetEnvironmentVariable("CookieRefreshTokenPath", EnvironmentVariableTarget.Process);
            RedisCacheSecurity = Convert.ToBoolean(System.Environment.GetEnvironmentVariable("RedisCacheSecurity", EnvironmentVariableTarget.Process));
        }

        public string SecretKey { get; private set; }
        public string CookieToken { get; private set; }
        public string CookieTokenPath { get; private set; }
        public string CookieRefreshToken { get; private set; }
        public string CookieRefreshTokenPath { get; private set; }
        public bool RedisCacheSecurity { get; private set; }

    }
}
