using AuthOpenId.Interfaces;
using AuthOpenId.Models;
using Bases.Interfaces;
using Bases.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace AuthOpenId.Services
{
    public class Social : ISocial
    {
        private readonly ISettingsTokens _settings;
        private readonly HttpClient client;
        private readonly IResult _result;
        private readonly IErrorMessages _errors;

        public Social(
            ISettingsTokens settings,
            IResult result,
            IErrorMessages errors
            )
        {
            _settings = settings;
            _result = result;
            _errors = errors;

            client = new HttpClient();
        }

        public async Task<Result<SocialModel>> GoogleValidateToken(dynamic data)
        {
            SocialModel model;
            try
            {
                var valid = true;

                string token = data.tokenId;
                token = token.Replace("{", string.Empty);
                token = token.Replace("}", string.Empty);
                Payload payload = await ValidateAsync(token, new ValidationSettings
                {
                    Audience = new[] { _settings.GoogleClientId }
                });

                if (!payload.Audience.Equals(_settings.GoogleClientId))
                    valid = false;
                if (!payload.Issuer.Equals(_settings.GoogleISSUER) && !payload.Issuer.Equals("https://" + _settings.GoogleISSUER))
                    valid = false;
                if (payload.ExpirationTimeSeconds == null)
                    valid = false;
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        valid = false;
                    }
                }

                if (!valid)
                {
                    return _result.Create<SocialModel>(false, _errors.NotAuthorized, null);
                }

                model = new SocialModel();
                model.Email = payload.Email;
                model.Name = payload.Name;
                model.Picture = payload.Picture;

            }
            catch (Exception)
            {
                return _result.Create<SocialModel>(false, _errors.NotAuthorized, null);
            }

            return _result.Create(true, "", model);
        }
        public async Task<Result<SocialModel>> FacebookValidateToken(dynamic data)
        {
            SocialModel model;
            try
            {
                string token = data.accessToken;

                HttpResponseMessage response = await client.GetAsync(_settings.GraphFacebook + data.userID + "?access_token=" + token);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                CallBackSocialModel facebook = JsonConvert.DeserializeObject<CallBackSocialModel>(responseBody);

                if (String.IsNullOrEmpty(facebook.error))
                {
                    model = new SocialModel();
                    model.Email = data.email;
                    model.Name = data.name;
                    model.Picture = data.picture.data.url;
                }
                else
                {
                    return _result.Create<SocialModel>(false, _errors.NotAuthorized, null);
                }
            }
            catch (Exception)
            {
                return _result.Create<SocialModel>(false, _errors.NotAuthorized, null);
            }

            return _result.Create(true, "", model);
        }
        public async Task<Result<SocialModel>> MicrosoftValidateToken(dynamic data)
        {
            SocialModel model;
            try
            {
                string token = data.accessToken;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(_settings.GraphMicrosoft);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                CallBackSocialModel microsoft = JsonConvert.DeserializeObject<CallBackSocialModel>(responseBody);

                if (String.IsNullOrEmpty(microsoft.error))
                {
                    model = new SocialModel();
                    model.Email = data.account.userName;
                    model.Name = data.account.name;
                }
                else
                {
                    return _result.Create<SocialModel>(false, _errors.NotAuthorized, null);
                }
            }
            catch (Exception)
            {
                return _result.Create<SocialModel>(false, _errors.NotAuthorized, null);
            }

            return _result.Create(true, "", model);
        }
    }
}
