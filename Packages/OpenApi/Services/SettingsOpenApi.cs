using Barracuda.OpenApi.Interfaces;
using System;

namespace Barracuda.OpenApi.Services
{
    public class SettingsOpenApi : ISettingsOpenApi
    {
        public SettingsOpenApi()
        {
            OAuthUrl = System.Environment.GetEnvironmentVariable("OAuthUrl", EnvironmentVariableTarget.Process);
            OAuthTokenUrl = System.Environment.GetEnvironmentVariable("OAuthTokenUrl", EnvironmentVariableTarget.Process);
            OAuthClientId = System.Environment.GetEnvironmentVariable("OAuthClientId", EnvironmentVariableTarget.Process);
            OAuthClientSecret = System.Environment.GetEnvironmentVariable("OAuthClientSecret", EnvironmentVariableTarget.Process);
            Scope = System.Environment.GetEnvironmentVariable("Scope", EnvironmentVariableTarget.Process);
            BOAUrlJson = System.Environment.GetEnvironmentVariable("BOAUrlJson", EnvironmentVariableTarget.Process);
            BOARedirectAuthUrl = System.Environment.GetEnvironmentVariable("BOARedirectAuthUrl", EnvironmentVariableTarget.Process);
            /*
            BarracudaAuthUrl = System.Environment.GetEnvironmentVariable("BarracudaAuthUrl", EnvironmentVariableTarget.Process);
            BarracudaRefreshTokenUrl = System.Environment.GetEnvironmentVariable("BarracudaRefreshTokenUrl", EnvironmentVariableTarget.Process);
            BarracudaRefreshUrl = System.Environment.GetEnvironmentVariable("BarracudaRefreshUrl", EnvironmentVariableTarget.Process);
            CookieToken = System.Environment.GetEnvironmentVariable("CookieToken", EnvironmentVariableTarget.Process);
            CookieTokenPath = System.Environment.GetEnvironmentVariable("CookieTokenPath", EnvironmentVariableTarget.Process);
            CookieRefreshToken = System.Environment.GetEnvironmentVariable("CookieRefreshToken", EnvironmentVariableTarget.Process);
            CookieRefreshTokenPath = System.Environment.GetEnvironmentVariable("CookieRefreshTokenPath", EnvironmentVariableTarget.Process);
       */
        }

        public string OAuthUrl { get; private set; }
        public string OAuthTokenUrl { get; private set; }
        public string OAuthClientId { get; private set; }
        public string OAuthClientSecret { get; private set; }
        public string Scope { get; private set; }
        public string BOAUrlJson { get; private set; }
        public string BOARedirectAuthUrl { get; private set; }
        /*
        public string BarracudaRefreshTokenUrl { get; private set; }
        public string BarracudaRefreshUrl { get; private set; }
        public string CookieToken { get; private set; }
        public string CookieTokenPath { get; private set; }
        public string CookieRefreshToken { get; private set; }
        public string CookieRefreshTokenPath { get; private set; }
        public string Scope { get; private set; }*/
        
    }
}
