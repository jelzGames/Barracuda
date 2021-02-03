using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Users.Application.Interfaces;
using System.Net.Http;
using Common.Models;
using System.Threading;
using System.Security.Claims;
using Users.Domain.Models;
using Bases.Interfaces;
using Barracuda.Indentity.Provider.Interfaces;
using Barracuda.Indentity.Provider.Services;

namespace Users.Functions
{
    public class UsersFunctions
    {
        private readonly IUserApplication _controller;
        private readonly IUserInfo _userInfo;
        private readonly IErrorMessagesExample _errors;

        public UsersFunctions(
            IUserApplication controller,
            IUserInfo userInfo,
             IErrorMessagesExample errors
        )
        {
            _controller = controller;
            _userInfo = userInfo;
            _errors = errors;
        }

        [FunctionName("CreateUser")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/Create")] HttpRequestMessage req,
            HttpRequest request, ILogger log)
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


            UserModel data = await req.Content.ReadAsAsync<UserModel>();

            var dataResult = await _controller.Create(data);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("GetUser")]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/Get/{id}")] HttpRequestMessage req, string id,
            HttpRequest request, ILogger log)
        {
            var resultAuth = validAuthorized(req, request);
            if (!resultAuth.Success)
            {

                return new BadRequestObjectResult(resultAuth.Message);
            }

            var dataResult = await _controller.Get(id);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("UpdateUser")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/Update")] HttpRequestMessage req,
            HttpRequest request, ILogger log)
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

            UserModel data = await req.Content.ReadAsAsync<UserModel>();

            var dataResult = await _controller.Update(data);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("GetAll")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/GetAll")] HttpRequestMessage req,
            HttpRequest request, ILogger log, CancellationToken cancellationToken)
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

            QueryInputModel data = await req.Content.ReadAsAsync<QueryInputModel>();
            
            var dataResult = await _controller.GetAll(data, cancellationToken);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/Delete/{id}")] HttpRequestMessage req, string id,
            HttpRequest request, ILogger log)
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

            var dataResult = await _controller.Delete(id);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("WarmFunctions")]
        public void Run([TimerTrigger("0 */4 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private Result<ClaimsPrincipal> validAuthorized(HttpRequestMessage req, HttpRequest request)
        {
            return _userInfo.ValidateTokenAsync(req.Headers, request.HttpContext.Connection.RemoteIpAddress);
        }
    }
}
