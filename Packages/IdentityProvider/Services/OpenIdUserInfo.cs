﻿using Barracuda.Indentity.Provider.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Services
{
    public class OpenIdUserInfo : IUserInfo
    {
        private readonly ISettingsTokens _settings;
        private readonly IResult _result;
        private readonly IErrorMessages _errors;

        ClaimsPrincipal principal;
        public string token;

        private readonly ITokens _tokens;

        public OpenIdUserInfo(
             ITokens tokens,
             ISettingsTokens settings,
              IResult result,
              IErrorMessages errors
        )
        {
            _tokens = tokens;
            _settings = settings;
            _result = result;
            _errors = errors;
        }

        public Result<ClaimsPrincipal> ValidateTokenAsync(HttpRequestHeaders headers, IPAddress ipAddress,
            List<string> scopes = null, List<string> tenants = null)
        {
            var flag = false;
            var token = "";
            foreach (var item in headers.GetValues("Cookie"))
            {
                var cookies = item.Split(",");
                for (var x = 0; x < cookies.Length; x++)
                {
                    var idx = cookies[x].IndexOf("=");
                    var name = cookies[x].Substring(0, idx);
                    var value = cookies[x].Substring(idx + 1, cookies[x].Length - (idx + 1));
                    if (name == _settings.CookieToken)
                    {
                        flag = true;
                        token = value;
                        break;
                    }
                }
            }

            if (!flag)
            {
                return _result.Create<ClaimsPrincipal>(false, _errors.NotAuthorized, null);
            }

            try
            {
                principal = _tokens.ValidateToken(token);

                if (principal != null && scopes != null)
                {
                    var resultScopes = validScopes(scopes);
                    if (!resultScopes.Success)
                    {
                        principal = null;
                    }
                }

                if (principal != null &&  tenants != null)
                {
                    var resulttenants = validTenants(tenants);
                    if (!resulttenants.Success)
                    {
                        principal = null;
                    }
                }
            }
            catch (SecurityTokenException ex)
            {
                if (ex.Message.Contains("Lifetime validation failed"))
                {
                    return _result.Create<ClaimsPrincipal>(false, _errors.SecurityTokenExpired, null);
                }
                else
                {
                    return _result.Create<ClaimsPrincipal>(false, _errors.NotAuthorized, null);
                }
            }
            catch (Exception)
            {
                return _result.Create<ClaimsPrincipal>(false, _errors.NotAuthorized, null);
            }

            if (principal == null)
            {
                return _result.Create<ClaimsPrincipal>(false, _errors.NotAuthorized, null);
            }

            return _result.Create(true, "", principal);
        }

        public Task<string> GetTokenAsync()
        {
            return Task.FromResult(token);
        }


        public Result<bool> validScopes(List<string> scopes)
        {
            foreach (var scope in this.Scopes)
            {
                if (scope == "BOAAdmin")
                {
                    return _result.Create(true, "", true);
                }
                else
                {
                    var idx = scopes.FindIndex((e) => e == scope);
                    if (idx > -1)
                    {
                        return _result.Create(true, "", true);
                    }
                }
            }

            return _result.Create(false, "", false);
        }

        public Result<bool> validTenants(List<string> tenants)
        {
            foreach (var tenant in this.Tenants)
            {
                var idx = tenants.FindIndex((e) => e == tenant);
                if (idx > -1)
                {
                    return _result.Create(true, "", true);
                }
            }

            return _result.Create(false, "", false);
        }

        /*
        public Result<bool> validScopes(dynamic scopes)
        {
            dynamic original = JsonConvert.DeserializeObject<dynamic>(this.Scopes);

            foreach (JProperty firstProperty in original)
            {
                var namefirstProperty = firstProperty.Name;
                var valuefirstProperty = firstProperty.Value;
                if (namefirstProperty == "superadmin")
                {
                    return _result.Create(true, "", true);
                }
                else if (valuefirstProperty.Children().ToList().Count > 0)
                {
                    foreach (JProperty secondProperty in valuefirstProperty)
                    {
                        var namesecondProperty = secondProperty.Name;
                        var valuesecondProperty = Convert.ToBoolean(secondProperty.Value);
                        try
                        {
                            if (scopes.PayLoad[namefirstProperty][namesecondProperty] != null &&
                                valuesecondProperty)
                            {
                                return _result.Create(true, "", true);
                            }
                        }
                        catch (RuntimeBinderException)
                        {
                        }
                    }
                }
            }
          
            return _result.Create(false, "", false);
        }
        */

        public string UserId
        {
            get
            {
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                return userId;
            }
        }

        public string Email
        {
            get
            {
                var userId = principal.FindFirst(ClaimTypes.Email).Value;
                return userId;
            }
        }

        public List<string> Scopes
        {
            get
            {
                List<string> scopes = JsonConvert.DeserializeObject<List<string>>(principal.FindFirst("Scopes").Value);
                return scopes;
            }
        }

        public List<string> Tenants
        {
            get
            {
                List<string> tenants = JsonConvert.DeserializeObject<List<string>>(principal.FindFirst("Tenants").Value);
                return tenants;
            }
        }
    }
}
