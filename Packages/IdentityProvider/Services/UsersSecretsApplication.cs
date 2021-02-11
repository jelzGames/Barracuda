using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Dtos;
using Barracuda.Indentity.Provider.Interfaces;
using Barracuda.Indentity.Provider.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IUserInfo _userInfo;

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
            ISettingsUserSecrests settingsSecrets,
            IUserInfo userInfo
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
            _userInfo = userInfo;
        }

        public async Task<Result<string>> Register(string userid, string email, string password, bool validEmail = false)
        {
            return await _services.Register(userid, email, password, validEmail);
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

        public LoginDto Logout(HttpRequest request)
        {
            EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, new UserPrivateDataDto());

            return new LoginDto();
        }

        public async Task<Result<LoginDto>> Refresh(string token, string refreshToken, HttpRequest request, bool remove = false)
        {
            var result = await ValidRefreshToken(token, refreshToken);
            if (!result.Success)
            {
                return _result.Create<LoginDto>(false, _errors.NotAuthorized, null);
            }

            if (!remove)
            {
                var dataResult = GetToken(result.Value);

                var login = EndToken(request, _settingsTokens.CookieToken, _settingsSecrets.CookieTokenPath, dataResult.Value);

                return _result.Create(true, "", login);
            }

            await DeleteRefreshToken(refreshToken);

            EndToken(request, _settingsSecrets.CookieRefreshToken, _settingsSecrets.CookieRefreshTokenPath, new UserPrivateDataDto());

            return _result.Create<LoginDto>(true, "", new LoginDto());
        }

        private async Task DeleteRefreshToken(string refreshToken)
        {
            var RefreshTokenHashed = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);

            if (!_settings.RedisCacheSecurity)
            {
                var result = await _services.GetSecrets(_userInfo.UserId);

                if (result.Success)
                {
                    var idx = result.Value.RefreshTokens.FindIndex((e) => e.Token == RefreshTokenHashed);
                    if (idx > -1)
                    {
                        result.Value.RefreshTokens.RemoveAt(idx);
                        await _services.Update(result.Value);
                    }
                }
            }
            else
            {
                List<RefreshTokensModel> queryTokens = new List<RefreshTokensModel>();
                var tokens = await _redisCache.GetSringValue(_userInfo.UserId);
                if (!String.IsNullOrEmpty(tokens))
                {
                    queryTokens = JsonConvert.DeserializeObject<List<RefreshTokensModel>>(tokens);
                    var idx = queryTokens.FindIndex((e) => e.Token == RefreshTokenHashed);
                    if (idx > -1)
                    {
                        queryTokens.RemoveAt(idx);
                        var jsonString = JsonConvert.SerializeObject(queryTokens);
                        await _redisCache.SetStringValue(_userInfo.UserId, jsonString);
                    }
                }
            }
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

            List<RefreshTokensModel> queryToken = null;
            UserPrivateDataModel model = null;

            if (!_settings.RedisCacheSecurity)
            {
                var result = await _services.GetSecrets(id);

                if (!result.Success)
                {
                    return _result.Create<UserPrivateDataModel>(false, result.Message, null);
                }

                queryToken = result.Value.RefreshTokens;
                model = result.Value;
            }
            else
            {
                var email = principal.FindFirst(ClaimTypes.Email).Value;
                var tokens = await _redisCache.GetSringValue(id);
                if (String.IsNullOrEmpty(tokens))
                {
                    return _result.Create<UserPrivateDataModel>(false, _errors.NotAuthorized, null);
                }

                queryToken = JsonConvert.DeserializeObject<List<RefreshTokensModel>>(tokens);

                model = new UserPrivateDataModel();
                model.id = id;
                model.Email = email;
            }

            var coderefreshToken = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);
            var idx = queryToken.FindIndex((e) => e.Token == coderefreshToken);
            if (idx < 0)
            {
                return _result.Create<UserPrivateDataModel>(false, _errors.NotAuthorized, null);
            }
            else
            {
                queryToken.RemoveAt(idx);
                if (!_settings.RedisCacheSecurity)
                {
                    model.RefreshTokens = queryToken;
                    await _services.Update(model);
                }
                else
                {
                    var jsonString = JsonConvert.SerializeObject(queryToken);
                    await _redisCache.SetStringValue(model.id, jsonString);
                }
            }
            
            return _result.Create(true, "", model);
        }
  
        private Result<UserPrivateDataDto> GetToken(UserPrivateDataModel model)
        {
            var token = _tokens.CreateToken(model.id, model.Email, model.Scopes, model.Tenants);

            var dto = new UserPrivateDataDto();
            dto.Id = model.id;
            dto.Email = model.Email;
            dto.ValidEmail = model.ValidEmail;
            dto.Token = token;
            dto.Scopes = model.Scopes;
            dto.Tenants = model.Tenants;

            return _result.Create(true, "", dto);
        }

        public async Task<Result<UserPrivateDataDto>> UpdateSecrets(UserPrivateDataModel model)
        {
            var refreshToken = _crypto.GetRandomNumber();
            var RefreshTokenHashed = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);
            createRefreshToken(model.id, model.RefreshTokens, RefreshTokenHashed);

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

        private void createRefreshToken(string id, List<RefreshTokensModel> refreshTokens, string RefreshTokenHashed)
        {
            if (refreshTokens == null)
            {
                refreshTokens = new List<RefreshTokensModel>();
            }

            if (refreshTokens.Count >= _settingsTokens.SessionsNumber)
            {
                var list = refreshTokens.OrderByDescending((e) => e.CreatedTime).ToList();
                list[0].Token = RefreshTokenHashed;
                list[0].CreatedTime = DateTime.UtcNow;
                refreshTokens = list;
            }
            else
            {
                refreshTokens.Add(new Models.RefreshTokensModel()
                {
                    Id = id,
                    Token = RefreshTokenHashed,
                    CreatedTime = DateTime.UtcNow
                });
            }
        }

        public async Task<Result<UserPrivateDataDto>> UpdateRedisCache(string id, string email)
        {
            List<RefreshTokensModel> queryTokens = new List<RefreshTokensModel>();
            var tokens = await _redisCache.GetSringValue(id);
            if (!String.IsNullOrEmpty(tokens))
            {
                queryTokens = JsonConvert.DeserializeObject<List<RefreshTokensModel>>(tokens);
            }

            var refreshToken = _crypto.GetRandomNumber();
            var RefreshTokenHashed = _crypto.GetStringSha256Hash(refreshToken + _settings.SecretKey);
            createRefreshToken(id, queryTokens, RefreshTokenHashed);

            var jsonString = JsonConvert.SerializeObject(queryTokens);

            await _redisCache.SetStringValue(id, jsonString);
            
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

            var register = await _services.Register(result.Value.id, result.Value.Email, _crypto.GetRandomNumber(), true);
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

            var register = await _services.Register(result.Value.id, result.Value.Email, _crypto.GetRandomNumber(), true);
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

            var register = await _services.Register(result.Value.id, result.Value.Email, _crypto.GetRandomNumber(), true);
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
            if(model.Block && DateTime.Now >= model.ExpirationLock)
            {
                model.Block = false;
            }
            return new LoginDto()
            {
                Id = model.Id,
                Email = model.Email,
                ValidEmail = model.ValidEmail,
                Scopes = model.Scopes,
                Tenants = model.Tenants,
                Block = model.Block
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

        public string ForgotPasswordOrRegister(string email)
        {
            return _tokens.CreateToken(Guid.NewGuid().ToString(), email, null, null, true);
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
        public string ValidateTokenConfirmEmail(string token)
        {
            var email = "";
            var principal = _tokens.ValidateToken(token, false);
            if (principal != null)
            {
                email = principal.FindFirst(ClaimTypes.Email).Value;
            }

            return email;
        }

        public async Task<Result<string>> UpdateScopes(string id, List<string> scopes)
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
        public async Task<Result<string>> ValidateRegisterEmail(string email)
        {
            return await _services.ValidateRegisterEmail(email);
        }

        public async Task<Result<string>> UpdateTenants(string id, List<string> tenants)
        {
            var result = await _services.GetSecrets(id);
            if (!result.Success)
            {
                return _result.Create<string>(false, result.Message, null);
            }

            result.Value.Tenants = tenants;

            var resultUpdate = await _services.Update(result.Value);
            if (!resultUpdate.Success)
            {
                return _result.Create<string>(false, resultUpdate.Message, null);
            }

            return _result.Create(true, "", "");
        }

        public async Task<Result<string>> DeleteUser(string id)
        {
            return await _services.DeleteUser(id);
        }

        public async Task<Result<string>> CheckEmail(string email)
        {
            return await _services.CheckEmail(email);
        }

        public async Task<Result<AdditionalModel>> GetAdditional(string id)
        {
            return await _services.GetAdditional(id);
        }

        public async Task<Result<string>> BlockUser(string id, bool Block)
        {
            var result = await _services.GetSecrets(id);
            if (!result.Success)
            {
                return _result.Create<string>(false, result.Message, null);
            }

            result.Value.Block = Block;

            var resultUpdate = await _services.BlockUser(result.Value);
            if (!resultUpdate.Success)
            {
                return _result.Create<string>(false, resultUpdate.Message, null);
            }

            return _result.Create(true, "", "");
        }
    }
}
