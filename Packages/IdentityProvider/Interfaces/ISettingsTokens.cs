﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ISettingsTokens
    {
        string IUsser { get; }
        string Audencie { get; }
        string JWTKey { get; }
        int SessionsNumber { get; }
        int ExpiredTimeInSeconds { get; }
        int ExpiredTimeInSecondsToForgetPasword { get; }
        int ExpiredTimeInSecondsToUserLocked { get; }
        string CookieToken { get; }
        string GoogleClientId { get; }
        string GoogleISSUER { get; }
        string GraphFacebook { get; }
        string GraphMicrosoft { get; }
        public string[] BarracudaSuperAdmins { get; }

    }
}
