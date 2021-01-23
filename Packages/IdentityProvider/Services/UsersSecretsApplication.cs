using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Dtos;
using Barracuda.Indentity.Provider.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Barracuda.Indentity.Provider.Services
{
    public class UsersSecretsApplication : IUsersSecretsApplication
    {
        private readonly IUsersSecretsDomain _services;
        private readonly ITokens _tokens;
        private readonly ISettingsUserSecrests _settings;
        private readonly ISocial _social;
        private readonly IRedisCache _redisCache;
        private readonly ICryptograhic _crypto;
        private readonly IResult _result;
        private readonly IErrorMessages _errors;
        private readonly ISettingsTokens _settingsTokens;
        private readonly ISettingsUserSecrests _settingsSecrets;

        public UsersSecretsApplication(
            IUsersSecretsDomain services,
            ITokens tokens,
            ISettingsUserSecrests settings,
            ISocial social,
            IRedisCache redisCache,
            ICryptograhic crypto,
            IResult result,
            IErrorMessages errors,
            ISettingsTokens settingsTokens,
            ISettingsUserSecrests settingsSecrets
        )
        {
            _services = services;
            _tokens = tokens;
            _settings = settings;
            _social = social;
            _redisCache = redisCache;
            _crypto = crypto;
            _result = result;
            _errors = errors;
            _settingsSecrets = settingsSecrets;
            _settingsTokens = settingsTokens;
        }

        public async Task<Result<string>> Register(string email, string password)
        {
            return await _services.Register(email, password);
        }

        public async Task<Result<LoginDto>> Login(string email, string password, HttpRequest request)
        {
            var result = await _services.Login(email);

            if (!result.Success)
            {
                return _result.Create<LoginDto>(false, result.Message, null);
            }

            var secret = _crypto.GetStringSha256Hash(email + password + _settings.SecretKey);
            if (result.Value.Password != secret)
            {
                return _result.Create<LoginDto>(false, _errors.NotAuthorized, null);
            }

            var dataResult = GetToken(result.Value);

            var login = EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, dataResult.Value);

            return _result.Create(true, "", login);
        }

        public async Task<Result<LoginDto>> Refresh(string token, string refreshToken, HttpRequest request)
        {
            var result = await ValidRefreshToken(token, refreshToken);
            if (!result.Success)
            {
                return _result.Create<LoginDto>(false, _errors.NotAuthorized, null);
            }

            var dataResult = GetToken(result.Value);

            var login = EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, dataResult.Value);

            return _result.Create(true, "", login);
        }

        public async Task<Result<LoginDto>> RefreshToken(string id, string email, HttpRequest request)
        {
            UserPrivateDataDto model = null;
            if (!_settings.RedisCacheSecurity)
            {
                var result = await _services.GetSecrets(id);

                if (!result.Success)
                {
                    return _result.Create<LoginDto>(false, result.Message, null);
                }

                var dataResult = await UpdateSecrets(result.Value);
                if (!dataResult.Success)
                {
                    return _result.Create<LoginDto>(false, dataResult.Message, null);
                }

                model = dataResult.Value;
            }
            else
            {
                var redishResult = await UpdateRedisCache(id, email);
                if (!redishResult.Success)
                {
                    return _result.Create<LoginDto>(false, redishResult.Message, null);
                }

                model = redishResult.Value;
            }

            var login = EndToken(request, _settingsSecrets.CookieRefreshToken, _settingsSecrets.CookieRefreshTokenPath, model);

            return _result.Create(true, "", login);
        }

        private async Task<Result<UserPrivateDataModel>> ValidRefreshToken(string token, string refreshToken)
        {
            var principal = _tokens.ValidateToken(token, false);
            var id = principal.FindFirst(ClaimTypes.NameIdentifier).Value;

            string queryToken = null;
            UserPrivateDataModel model = null;

            if (!_settings.RedisCacheSecurity)
            {
                var result = await _services.GetSecrets(id);

                if (!result.Success)
                {
                    return _result.Create<UserPrivateDataModel>(false, result.Message, null);
                }

                queryToken = result.Value.RefreshToken;
                model = result.Value;
            }
            else
            {
                var email = principal.FindFirst(ClaimTypes.Email).Value;
                queryToken = await _redisCache.GetSringValue(id);
                if (String.IsNullOrEmpty(queryToken))
                {
                    return _result.Create<UserPrivateDataModel>(false, _errors.NotAuthorized, null);
                }

                model = new UserPrivateDataModel();
                model.id = id;
                model.Email = email;
            }

            var coderefreshToken = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);

            if (queryToken != coderefreshToken)
            {
                return _result.Create<UserPrivateDataModel>(false, _errors.NotAuthorized, null);
            }


            return _result.Create(true, "", model);
        }

        private Result<UserPrivateDataDto> GetToken(UserPrivateDataModel model)
        {
            var token = _tokens.CreateToken(model.id, model.Email, model.Scopes);

            var dto = new UserPrivateDataDto();
            dto.Id = model.id;
            dto.Email = model.Email;
            dto.Token = token;

            return _result.Create(true, "", dto);
        }

        public async Task<Result<UserPrivateDataDto>> UpdateSecrets(UserPrivateDataModel model)
        {
            var refreshToken = _crypto.GetRandomNumber();
            model.RefreshToken = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);

            var result = await _services.Update(model);
            if (!result.Success)
            {
                return _result.Create<UserPrivateDataDto>(false, result.Message, null);
            }

            var dto = new UserPrivateDataDto();
            dto.Id = model.id;
            dto.Email = model.Email;
            dto.Token = refreshToken;

            return _result.Create(true, "", dto);

        }

        public async Task<Result<UserPrivateDataDto>> UpdateRedisCache(string id, string email)
        {
            var refreshToken = _crypto.GetRandomNumber();
            var redisToken = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);

            await _redisCache.SetStringValue(id, redisToken);

            var dto = new UserPrivateDataDto();
            dto.Id = id;
            dto.Email = email;
            dto.Token = refreshToken;

            return _result.Create(true, "", dto);

        }

        public async Task<Result<LoginDto>> GoogleValidateToken(dynamic data, HttpRequest request)
        {
            var result = await _social.GoogleValidateToken(data);
            if (!result.Success)
            {
                return _result.Create<UserPrivateDataDto>(false, result.Message, null);
            }

            var register = await _services.Register(result.Value.Email, _crypto.GetRandomNumber());
            if (!register.Success)
            {
                if (register.Message != _errors.Found)
                {
                    return _result.Create<LoginDto>(false, register.Message, null);
                }
            }

            UserPrivateDataModel model = new UserPrivateDataModel();
            model.id = register.Value;
            model.Email = result.Value.Email;

            var dataResult = GetToken(model);

            var login = EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, dataResult.Value);

            return _result.Create(true, "", login);
        }
        public async Task<Result<LoginDto>> FacebookValidateToken(dynamic data, HttpRequest request)
        {
            var result = await _social.FacebookValidateToken(data);
            if (!result.Success)
            {
                return _result.Create<LoginDto>(false, result.Message, null);
            }

            var register = await _services.Register(result.Value.Email, _crypto.GetRandomNumber());
            if (!register.Success)
            {
                if (register.Message != _errors.Found)
                {
                    return _result.Create<LoginDto>(false, register.Message, null);
                }
            }

            UserPrivateDataModel model = new UserPrivateDataModel();
            model.id = register.Value;
            model.Email = result.Value.Email;

            var dataResult = GetToken(model);

            var login = EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, dataResult.Value);

            return _result.Create(true, "", login);
        }

        public async Task<Result<LoginDto>> MicrosoftValidateToken(dynamic data, HttpRequest request)
        {
            var result = await _social.MicrosoftValidateToken(data);
            if (!result.Success)
            {
                return _result.Create<LoginDto>(false, result.Message, null);
            }

            var register = await _services.Register(result.Value.Email, _crypto.GetRandomNumber());
            if (!register.Success)
            {
                if (register.Message != _errors.Found)
                {
                    return _result.Create<LoginDto>(false, register.Message, null);
                }
            }

            UserPrivateDataModel model = new UserPrivateDataModel();
            model.id = register.Value;
            model.Email = result.Value.Email;

            var dataResult = GetToken(model);

            var login = EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, dataResult.Value);

            return _result.Create(true, "", login);
        }

        public LoginDto EndToken(HttpRequest request, string cookie, string path, UserPrivateDataDto model)
        {
            SetCookie(request.HttpContext.Response.Cookies, cookie, model.Token, path);

            return new LoginDto()
            {
                Id = model.Id,
                Email = model.Email
            };
        }

        private void SetCookie(IResponseCookies cookies, string name, string token, string path)
        {
            cookies.Append(
             name,
             token,
             new CookieOptions
             {
                 Path = path,
                 HttpOnly = true,
                 Secure = true,
                 SameSite = SameSiteMode.Strict
             });
        }

        public TokensDto ValidateToken(HttpRequestMessage req)
        {
            var token = "";
            var refreshToken = "";
            foreach (var item in req.Headers.GetValues("Cookie"))
            {
                var cookies = item.Split(",");
                for (var x = 0; x < cookies.Length; x++)
                {
                    var idx = cookies[x].IndexOf("=");
                    var name = cookies[x].Substring(0, idx);
                    var value = cookies[x].Substring(idx + 1, cookies[x].Length - (idx + 1));
                    if (name == _settingsTokens.CookieToken)
                    {
                        token = value;
                    }
                    else if (name == _settingsSecrets.CookieRefreshToken)
                    {
                        refreshToken = Uri.UnescapeDataString(value);
                    }
                }
            }

            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(refreshToken))
            {
                return new TokensDto()
                {
                    Valid = false
                };
            }

            return new TokensDto()
            {
                Token = token,
                RefreshToken = refreshToken,
                Valid = true
            };
        }

        public async Task<Result<string>> ChangePassword(string email, string password)
        {
            return await _services.ChangePassword(email, password);
        }

        public string ForgotPassword(string email)
        {
            return _tokens.CreateToken(Guid.NewGuid().ToString(), email, null, true);
        }

        public string ValidateToken(string token)
        {
            var email = "";
            var principal = _tokens.ValidateToken(token);
            if (principal != null)
            {
                email = principal.FindFirst(ClaimTypes.Email).Value;
            }

            return email;
        }

        public async Task<Result<string>> UpdateScopes(string id, dynamic scopes)
        {
            var result = await _services.GetSecrets(id);
            if (!result.Success)
            {
                return _result.Create<string>(false, result.Message, null);
            }

            result.Value.Scopes = scopes; 

            var resultUpdate = await _services.Update(result.Value);
            if (!resultUpdate.Success)
            {
                return _result.Create<string>(false, resultUpdate.Message, null);
            }

            return _result.Create(true, "", "");
        }
    }
}
