using Barracuda.Indentity.Provider.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Barracuda.Indentity.Provider.Services
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

        public string CreateToken(string id, string email, List<string> userScopes, List<string> userTenants, bool iSforgotPasswordOrRegister = false)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JWTKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
            {
            };

            var seconds = _settings.ExpiredTimeInSeconds;
            if (iSforgotPasswordOrRegister)
            {
                seconds = _settings.ExpiredTimeInSecondsToForgetPasword;
            }

            if (userScopes == null)
            {
                userScopes = new List<string>();    
            }

            if (userTenants == null)
            {
                userTenants = new List<string>();
            }

            var idx = Array.FindIndex(_settings.BarracudaSuperAdmins, (e) => e == email);
            if (idx > -1)
            {
                userScopes.Add("BOAAdmin");
            }

            var timeExpire = DateTime.UtcNow.AddSeconds(seconds);

            var secToken = new JwtSecurityToken(
                _settings.IUsser,
                _settings.Audencie,
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, id),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim("Scopes", JsonConvert.SerializeObject(userScopes)),
                    new Claim("Tenants", JsonConvert.SerializeObject(userTenants))
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

            ClaimsPrincipal principal = null;
            try
            {
                principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            }
            catch(SecurityTokenException ex)
            {
                throw ex;
            }

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
                model.LifetimeValidator =  LifetimeValidator;

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
