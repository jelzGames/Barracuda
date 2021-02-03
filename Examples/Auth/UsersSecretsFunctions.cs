using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Security.Claims;
using Barracuda.Indentity.Provider.Interfaces;
using Barracuda.Indentity.Provider.Services;
using Barracuda.Indentity.Provider.Dtos;
using System.Collections.Generic;

namespace UsersSecrets.Functions
{
    public class UsersSecretsFunctions
    {
        private readonly IUsersSecretsApplication _controller;
        private readonly IUserInfo _userInfo;
        private readonly ISettingsUserSecrests _commonSettings;
        private readonly ISettingsTokens _settingsTokens;
        private readonly IErrorMessages _errors;
        
        public UsersSecretsFunctions(
            IUsersSecretsApplication controller,
            IUserInfo userInfo,
            ISettingsUserSecrests commonSettings,
            IErrorMessages errors,
            ISettingsTokens settingsTokens
        )
        {
            _commonSettings = commonSettings;
            _controller = controller;
            _userInfo = userInfo;
            _errors = errors;
            _settingsTokens = settingsTokens;
        }

        [FunctionName("Register")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/Register")] HttpRequestMessage req,
            ILogger log)
        {
            UsersSecretsDto data = await req.Content.ReadAsAsync<UsersSecretsDto>();

            var email = data.Email == null ? "" : data.Email;
            var password = data.Password == null ? "" : data.Password;

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.Register(data.Email, data.Password);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            // optional
            var dataScope = await _controller.UpdateScopes(dataResult.Value, new
            {
                blobs = new
                {
                    update = true,
                    read = true
                }
            });

            // optional
            // tenants can be grouped by example: "mycompany/surcusals" group is first element and child second
            // using * means all, if you are grouping that refrence to all with the same group by example "mycompany/*"  
            var dataTenants = await _controller.UpdateTenants(dataResult.Value, new List<string>() { "mycompany/*", "mycompany/surcusals" });

            var token = _controller.ForgotPasswordOrRegister(email);

            // you can build you custom address with the token generated 
            // example https://mysite.com/onRoute/?validEmailToken=" + token
            // and send an email to requester including the address in the email body 

            // you can call to change password api whit the token recived from email
            // exmaple: https://localhost/api/permissions/ValidEmail?token=" + token

            // It is important use the parameter named token

            // Remember not return the token only if has been calling internally by the server
            // use return new OkResult(); instead


            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("Scopes")]
        public async Task<IActionResult> UpdateScopes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/UpdateScopes")] HttpRequestMessage req,
            ILogger log, HttpRequest request)
        {
            var resultAuth = validAuthorized(req, request);
            if (!resultAuth.Success)
            {
                return new UnauthorizedResult();
            }

            var resultScopes = validScopes(new { 
                superamdmin = true,
                scopes = new {
                    update = true
                }
            });
            if (!resultScopes.Success)
            {
                return new UnauthorizedResult();
            }

            LoginDto data = await req.Content.ReadAsAsync<LoginDto>();

            var id = data.Id == null ? "" : data.Id;
           
            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.UpdateScopes(data.Id, data.Scopes);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("Tenants")]
        public async Task<IActionResult> UpdateTenants(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/UpdateTenants")] HttpRequestMessage req,
            ILogger log, HttpRequest request)
        {
            var resultAuth = validAuthorized(req, request);
            if (!resultAuth.Success)
            {
                return new UnauthorizedResult();
            }

            var resultScopes = validScopes(new
            {
                superamdmin = true,
                scopes = new
                {
                    update = true
                }
            });
            if (!resultScopes.Success)
            {
                return new UnauthorizedResult();
            }

            LoginDto data = await req.Content.ReadAsAsync<LoginDto>();

            var id = data.Id == null ? "" : data.Id;

            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.UpdateScopes(data.Id, data.Tenants);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("Auth")]
        public async Task<IActionResult> Auth(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/Auth")] HttpRequestMessage req,
            HttpRequest request, ILogger log)
        {
            UsersSecretsDto data = await req.Content.ReadAsAsync<UsersSecretsDto>();

            var email = data.Email == null ? "" : data.Email;
            var password = data.Password == null ? "" : data.Password;

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            var dataResult = await _controller.Login(data.Email, data.Password, request);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("RefreshToken")]
        public async Task<IActionResult> RefreshToken(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/RefreshToken")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var resultAuth = validAuthorized(req, request);
            if (!resultAuth.Success)
            {
                return new UnauthorizedResult();
            }

            var dataResult = await _controller.RefreshToken(_userInfo.UserId, _userInfo.Email, request);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("Refresh")]
        public async Task<IActionResult> Refresh(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/Refresh")] HttpRequestMessage req,
            HttpRequest request, ILogger log)
        {
            var validToken = _controller.ValidateToken(req);
            if (!validToken.Valid)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            var dataResult = await _controller.Refresh(validToken.Token, validToken.RefreshToken, request);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("SocialGoogle")]
        public async Task<IActionResult> SocialGoogle(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/SocialGoogle")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var data = await req.Content.ReadAsAsync<dynamic>();

            var dataResult = await _controller.GoogleValidateToken(data, request);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("SocialFacebook")]
        public async Task<IActionResult> SocialFacebook(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/SocialFacebook")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var data = await req.Content.ReadAsAsync<dynamic>();

            var dataResult = await _controller.FacebookValidateToken(data, request);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("SocialMicrosoft")]
        public async Task<IActionResult> SocialMicrosoft(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/SocialMicrosoft")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var data = await req.Content.ReadAsAsync<dynamic>();

            var dataResult = await _controller.MicrosoftValidateToken(data, request);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("ChangePassword")]
        public async Task<IActionResult> ChangePassword(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/ChangePassword")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var token = req.RequestUri.ParseQueryString().Get("changepasswordtoken");
            var emailToken = "";
            if (!String.IsNullOrEmpty(token))
            {
                emailToken = _controller.ValidateToken(token);
                if (String.IsNullOrEmpty(emailToken))
                {
                    return new BadRequestObjectResult(_errors.NotAuthorized);
                }
            }
            else
            {
                var resultAuth = validAuthorized(req, request);
                if (!resultAuth.Success)
                {
                    if (resultAuth.Message == _errors.NotAuthorized)
                    {
                        return new UnauthorizedResult();
                    }
                    else
                    {
                        var objectResult = new ObjectResult(resultAuth.Message)
                        {
                            StatusCode = StatusCodes.Status401Unauthorized
                        };

                        return objectResult;
                    }
                }
            }

            UsersSecretsDto data = await req.Content.ReadAsAsync<UsersSecretsDto>();

            var email = data.Email == null ? "" : data.Email;
            var password = data.Password == null ? "" : data.Password;

            if (!String.IsNullOrEmpty(emailToken))
            {
            
                email = emailToken;
            }
            

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.ChangePassword(email, password);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("ForgotPassword")]
        public IActionResult ForgotPassword(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "permissions/ForgotPassword")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {

            var email = req.RequestUri.ParseQueryString().Get("email");
            email = email == null ? "" : email;
         
            if (String.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var token = _controller.ForgotPasswordOrRegister(email);


            // you can buid you custom address with the token generated 
            // example https://mysite.com/onRoute/?changepasswordtoken=" + token
            // and send an email to requester including the address in the email body 

            // you can call to change password api whit the token recived from email
            // exmaple: https://localhost/api/permissions/ChangePassword?token=" + token

            // It is important use the parameter named token

            // Remember not return the token only if has been calling internally by the server
            // use return new OkResult(); instead

            return new OkObjectResult("Ok");
        }

        [FunctionName("ValidEmail")]
        public async Task<IActionResult> ValidEmail(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/ValidEmail")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var token = req.RequestUri.ParseQueryString().Get("validEmailToken");
            var emailToken = "";
            if (!String.IsNullOrEmpty(token))
            {      
                emailToken = _controller.ValidateToken(token);
                
                if (String.IsNullOrEmpty(emailToken))
                {
                    return new BadRequestObjectResult(_errors.ValidateTokenConfirmEmailExpired);
                }
            }

            var dataResult = await _controller.ValidateRegisterEmail(emailToken);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("ResendValidEmail")]
        public IActionResult ResendValidEmail(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/ResendValidEmail")] HttpRequestMessage req,
          HttpRequest request, ILogger log)
        {
            var token = req.RequestUri.ParseQueryString().Get("validEmailToken");
            var resendEmail = req.RequestUri.ParseQueryString().Get("email");
            var emailToken = "";
            if (!String.IsNullOrEmpty(token))
            {
                emailToken = _controller.ValidateTokenConfirmEmail(token);
            }
            else
            {
                if (resendEmail != null)
                {
                    emailToken = resendEmail;
                }
            }
            if (String.IsNullOrEmpty(emailToken))
            {
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            var newToken = _controller.ForgotPasswordOrRegister(emailToken);

            // you can build you custom address with the token generated 
            // example https://mysite.com/onRoute/?validEmailToken=" + token
            // and send an email to requester including the address in the email body 

            // you can call to change password api whit the token recived from email
            // exmaple: https://localhost/api/permissions/ValidEmail?token=" + token

            // It is important use the parameter named token

            // Remember not return the token only if has been calling internally by the server
            // use return new OkResult(); instead


            return new OkObjectResult("OK");
        }

        private Result<ClaimsPrincipal> validAuthorized(HttpRequestMessage req, HttpRequest request)
        {
            return _userInfo.ValidateTokenAsync(req.Headers, request.HttpContext.Connection.RemoteIpAddress);
        }

        private Result<bool> validScopes(dynamic scopes)
        {
  
            return _userInfo.validScopes(scopes);
        }

        [FunctionName("WarmFunctions")]
        public void Run([TimerTrigger("0 */4 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
