﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Interfaces
{
    public interface ISettingsOpenApi
    {
        public string OAuthUrl { get; }
        public string OAuthTokenUrl { get; }
        public string OAuthClientId { get; }
        public string OAuthClientSecret { get; }
        public string Scope { get; }
        public string BOAUrlJson { get; }
        public string BOARedirectAuthUrl { get; }

        public string BarracudaAuthUrl { get; }
        public string BarracudaRefreshTokenUrl { get; }
        public string BarracudaRefreshUrl { get; }
        public string CookieToken { get; }
        public string CookieTokenPath { get; }
        public string CookieRefreshToken { get; }
        public string CookieRefreshTokenPath { get; }
        public string BarracudaFunctionsUrlRoot { get; }
        public string BarracudaLogouthUrl { get; }
        public string BarracudaRemoveRefreshTokenhUrl { get; }
        public string BarracudaPostMessages { get; }
        public string SecretKey { get; }
        public int SessionsNumber { get; }
        public int ExpiredTimeInSeconds { get; }
        public string[] BarracudaSuperAdmins { get; }
    }
}
