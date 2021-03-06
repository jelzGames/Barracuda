﻿using AuthOpenId.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthOpenId.Services
{
    public class Tokens : ITokens
    {
        private readonly ISettingsTokens _settings;

        public Tokens(
             ISettingsTokens settings
        )
        {
            _settings = settings;
        }

        public string CreateToken(string id, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JWTKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
            {
            };

            var timeExpire = DateTime.UtcNow.AddSeconds(_settings.ExpiredTimeInSeconds);

            var secToken = new JwtSecurityToken(
                _settings.IUsser,
                _settings.Audencie,
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, id),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                },
                null,
                timeExpire,
                credentials
            );

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public ClaimsPrincipal ValidateToken(string authToken, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(validateLifetime);

            SecurityToken validatedToken;

            ClaimsPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);

            return principal;
        }

        private TokenValidationParameters GetValidationParameters(bool validateLifetime = true)
        {
            var model = new TokenValidationParameters()
            {
                ValidateLifetime = validateLifetime,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _settings.IUsser,
                ValidAudience = _settings.Audencie,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JWTKey))
            };

            if (validateLifetime)
            {
                model.LifetimeValidator = LifetimeValidator;

            }

            return model;
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }

    }
}
