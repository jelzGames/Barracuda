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
using Newtonsoft.Json;

namespace UsersSecrets.Functions
{
    public class UsersSecretsFunctions
    {
        private readonly IUsersSecretsApplication _controller;
        private readonly IUserInfo _userInfo;
        private readonly ISettingsUserSecrests _commonSettings;
        private readonly ISettingsTokens _settingsTokens;
        private readonly IErrorMessages _errors;
        private readonly IResult _result;

        public UsersSecretsFunctions(
            IUsersSecretsApplication controller,
            IUserInfo userInfo,
            ISettingsUserSecrests commonSettings,
            IErrorMessages errors,
            ISettingsTokens settingsTokens,
            IResult result
        )
        {
            _commonSettings = commonSettings;
            _controller = controller;
            _userInfo = userInfo;
            _errors = errors;
            _settingsTokens = settingsTokens;
            _result = result;
        }

        /// <summary>
        /// Single sign on (SSO)
        /// </summary>
       
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

            var dataResult = await _controller.Register(data.Id, data.Email, data.Password);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            // optional
            var dataScope = await _controller.UpdateScopes(dataResult.Value, new List<string> { "users.read" });
            
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

        [FunctionName("Login")]
        public async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/Login")] HttpRequestMessage req,
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
                if (dataResult.Message.Contains("Block"))
                {
                    return new BadRequestObjectResult(dataResult.Message);
                }
                return new BadRequestObjectResult(_errors.NotAuthorized);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("Logout")]
        public IActionResult Logout(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "permissions/Logout")] HttpRequestMessage req,
           HttpRequest request, ILogger log)
        {
            var resultAuth = validAuthorized(req, request);
            if (!resultAuth.Success)
            {
                return new UnauthorizedResult();
            }
            
            var logout = _controller.Logout(request);

            return new OkObjectResult(logout);
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

            var remove = req.RequestUri.ParseQueryString().Get("remove");
            var flag = remove == null ? false : Convert.ToBoolean(remove); 

            var dataResult = await _controller.Refresh(validToken.Token, validToken.RefreshToken, request, flag);

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
            //opcional
            data.Scopes = JsonConvert.SerializeObject(new List<string>() { "user.read" });
            data.Tenants = JsonConvert.SerializeObject(new List<string>() { "mycompany/sucursal" });

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
            //opcional
            data.Scopes = JsonConvert.SerializeObject(new List<string>() { "user.read" });
            data.Tenants = JsonConvert.SerializeObject(new List<string>() { "mycompany/sucursal" });

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
            //opcional
            data.Scopes = JsonConvert.SerializeObject(new List<string>() { "user.read" });
            data.Tenants = JsonConvert.SerializeObject(new List<string>() { "mycompany/sucursal" });

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

        /// <summary>
        /// End Single sign on (SSO)
        /// </summary>

        /// <summary>
        /// Administrative
        /// </summary>

        [FunctionName("AddUser")]
        public async Task<IActionResult> AddUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/AddUser")] HttpRequestMessage req,
            ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            UsersSecretsDto data = await req.Content.ReadAsAsync<UsersSecretsDto>();

            var email = data.Email == null ? "" : data.Email;
            var password = data.Password == null ? "" : data.Password;

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.Register(data.Id, data.Email, data.Password, true);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "permissions/DeleteUser/{id}")] HttpRequestMessage req,
            string id, ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            var dataResult = await _controller.DeleteUser(id);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("UpdateScopes")]
        public async Task<IActionResult> UpdateScopes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/UpdateScopes")] HttpRequestMessage req,
            ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
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

        [FunctionName("UpdateTenants")]
        public async Task<IActionResult> UpdateTenants(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/UpdateTenants")] HttpRequestMessage req,
            ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            LoginDto data = await req.Content.ReadAsAsync<LoginDto>();

            var id = data.Id == null ? "" : data.Id;

            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.UpdateTenants(data.Id, data.Tenants);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("ChangePasswordToUser")]
        public async Task<IActionResult> ChangePasswordToUser(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/ChangePasswordToUser")] HttpRequestMessage req,
          HttpRequest request, ILogger log)
        {
        
            UsersSecretsDto data = await req.Content.ReadAsAsync<UsersSecretsDto>();

            var email = data.Email == null ? "" : data.Email;
            var password = data.Password == null ? "" : data.Password;

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            var dataResult = await _controller.ChangePassword(email, password);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("GetAdditional")]
        public async Task<IActionResult> GetAdditional(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "permissions/GetAdditional/{id}")] HttpRequestMessage req,
          string id, ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.GetAdditional(id);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("GetBatchAdditional")]
        public async Task<IActionResult> GetBatchAdditional(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/GetBatchAdditional")] HttpRequestMessage req,
           ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.update" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            AdditionaltBatchDto data = await req.Content.ReadAsAsync<AdditionaltBatchDto>();
            
            var dataResult = await _controller.GetBatchAdditional(data.Batch);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("BlockUser")]
        public async Task<IActionResult> BlockUser(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "permissions/BlockUser")] HttpRequestMessage req,
           ILogger log, HttpRequest request)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "admin.block" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            LoginDto data = await req.Content.ReadAsAsync<LoginDto>();

            var id = data.Id == null ? "" : data.Id;

            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.BlockUser(data.Id, data.Block);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }
        /// <summary>
        /// end administrative
        /// </summary>

        /// <summary>
        /// Generic
        /// </summary>

        [FunctionName("CheckEmail")]
        public async Task<IActionResult> CheckEmail(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "permissions/CheckEmail/{email}")] HttpRequestMessage req,
         HttpRequest request, ILogger log, string email)
        {

            if (String.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult(_errors.ValuesNotValid);
            }

            var dataResult = await _controller.CheckEmail(email);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        /// <summary>
        /// Generic end
        /// </summary>
        private Result<ClaimsPrincipal> validAuthorized(HttpRequestMessage req, HttpRequest request, List<string> scopes = null, List<string> tenants = null)
        {
            return _userInfo.ValidateTokenAsync(req.Headers, request.HttpContext.Connection.RemoteIpAddress, scopes, tenants);
        }

        private Result<bool> validAdmin(HttpRequestMessage req, HttpRequest request, List<string> scopes = null, List<string> tenants = null)
        {
            var resultAuth = validAuthorized(req, request, scopes, tenants);
            if (!resultAuth.Success)
            {
                return _result.Create<bool>(false,resultAuth.Message,false);
            }

            return _result.Create<bool>(true, "" , true);
        }

        [FunctionName("WarmFunctions")]
        public void Run([TimerTrigger("0 */4 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
