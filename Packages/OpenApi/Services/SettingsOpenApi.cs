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

            var admins = System.Environment.GetEnvironmentVariable("BarracudaSuperAdmins", EnvironmentVariableTarget.Process);
            if (admins != null)
            {
                BarracudaSuperAdmins = admins.Split(",");
            }
            else
            {
                BarracudaSuperAdmins = new string[0];
            }
            SecretKey = System.Environment.GetEnvironmentVariable("SecretKey", EnvironmentVariableTarget.Process);
            SessionsNumber = Convert.ToInt32(System.Environment.GetEnvironmentVariable("SessionsNumber", EnvironmentVariableTarget.Process));
            ExpiredTimeInSeconds = Convert.ToInt32(System.Environment.GetEnvironmentVariable("ExpiredTimeInSeconds",
               EnvironmentVariableTarget.Process));
            BarracudaAuthUrl = System.Environment.GetEnvironmentVariable("BarracudaAuthUrl", EnvironmentVariableTarget.Process);
            BarracudaRefreshTokenUrl = System.Environment.GetEnvironmentVariable("BarracudaRefreshTokenUrl", EnvironmentVariableTarget.Process);
            BarracudaRefreshUrl = System.Environment.GetEnvironmentVariable("BarracudaRefreshUrl", EnvironmentVariableTarget.Process);
            BarracudaFunctionsUrlRoot = System.Environment.GetEnvironmentVariable("BarracudaFunctionsUrlRoot", EnvironmentVariableTarget.Process);
            BarracudaLogouthUrl = System.Environment.GetEnvironmentVariable("BarracudaLogouthUrl", EnvironmentVariableTarget.Process);
            BarracudaRemoveRefreshTokenhUrl = System.Environment.GetEnvironmentVariable("BarracudaRemoveRefreshTokenhUrl", EnvironmentVariableTarget.Process);
            BarracudaRemoveRefreshTokenhUrl = System.Environment.GetEnvironmentVariable("BarracudaRemoveRefreshTokenhUrl", EnvironmentVariableTarget.Process);
            CookieToken = System.Environment.GetEnvironmentVariable("CookieToken", EnvironmentVariableTarget.Process);
            CookieTokenPath = System.Environment.GetEnvironmentVariable("CookieTokenPath", EnvironmentVariableTarget.Process);
            CookieRefreshToken = System.Environment.GetEnvironmentVariable("CookieRefreshToken", EnvironmentVariableTarget.Process);
            BarracudaPostMessages = System.Environment.GetEnvironmentVariable("BarracudaPostMessages", EnvironmentVariableTarget.Process);

        }

        public string OAuthUrl { get; private set; }
        public string OAuthTokenUrl { get; private set; }
        public string OAuthClientId { get; private set; }
        public string OAuthClientSecret { get; private set; }
        public string Scope { get; private set; }
        public string BOAUrlJson { get; private set; }
        public string BOARedirectAuthUrl { get; private set; }
        public string BarracudaRefreshTokenUrl { get; private set; }
        public string BarracudaRefreshUrl { get; private set; }
        public string CookieToken { get; private set; }
        public string CookieTokenPath { get; private set; }
        public string CookieRefreshToken { get; private set; }
        public string CookieRefreshTokenPath { get; private set; }
        public string BarracudaAuthUrl { get; private set; }
        public string BarracudaFunctionsUrlRoot { get; private set; }
        public string BarracudaLogouthUrl { get; private set; }
        public string BarracudaRemoveRefreshTokenhUrl { get; private set; }
        public string BarracudaPostMessages { get; private set; }
        public string SecretKey { get; private set; }
        public int SessionsNumber { get; private set; }
        public int ExpiredTimeInSeconds { get; private set; }
        public string[] BarracudaSuperAdmins { get; private set; }
    }
}
