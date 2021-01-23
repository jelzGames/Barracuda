using AuthOpenId.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthOpenId.Services
{
    public class SettingsTokens : ISettingsTokens
    {
        public SettingsTokens()
        {
            IUsser = System.Environment.GetEnvironmentVariable("IUsser", EnvironmentVariableTarget.Process);
            Audencie = System.Environment.GetEnvironmentVariable("Audencie", EnvironmentVariableTarget.Process);
            JWTKey = System.Environment.GetEnvironmentVariable("JWTKey", EnvironmentVariableTarget.Process);
            ExpiredTimeInSeconds = Convert.ToInt32(System.Environment.GetEnvironmentVariable("ExpiredTimeInSeconds",
                EnvironmentVariableTarget.Process));
            CookieToken = System.Environment.GetEnvironmentVariable("CookieToken", EnvironmentVariableTarget.Process);
            GoogleClientId = System.Environment.GetEnvironmentVariable("GoogleClientId", EnvironmentVariableTarget.Process);
            GoogleISSUER = System.Environment.GetEnvironmentVariable("GoogleISSUER", EnvironmentVariableTarget.Process);
            GraphFacebook = System.Environment.GetEnvironmentVariable("GraphFacebook", EnvironmentVariableTarget.Process);
            GraphMicrosoft = System.Environment.GetEnvironmentVariable("GraphMicrosoft", EnvironmentVariableTarget.Process);
        }

        public string IUsser { get; private set; }
        public string Audencie { get; private set; }
        public string JWTKey { get; private set; }
        public int ExpiredTimeInSeconds { get; private set; }
        public string CookieToken { get; private set; }
        public string GoogleClientId { get; private set; }
        public string GoogleISSUER { get; private set; }
        public string GraphMicrosoft { get; private set; }
        public string GraphFacebook { get; private set; }

    }
}
