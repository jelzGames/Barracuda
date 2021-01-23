using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using Blobs.Application.Interfaces;
using Blobs.Application.Dtos;
using Blobs.Domain.Models;
using System.Threading;
using Common.Models;
using System.Security.Claims;
using Bases.Services;
using Bases.Interfaces;
using AuthOpenId.Interfaces;

namespace Blobs.Functions
{
    public class BlobFunctions
    {
        private readonly ISettingsBlobs _settings;
        private readonly IUserInfo _userInfo;
        private readonly IBlobsApplication _controller;
        private readonly IErrorMessages _errors;

        public BlobFunctions(
            ISettingsBlobs settings,
            IUserInfo userInfo,
            IBlobsApplication controller,
             IErrorMessages errors
            )
        {
            _settings = settings;
            _userInfo = userInfo;
            _controller = controller;
            _errors = errors;
           
        }

        [FunctionName("CreateBlob")]
        public async Task<IActionResult> CreateBlob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blobs/Create")] HttpRequestMessage req,
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

            BlobModel data = await req.Content.ReadAsAsync<BlobModel>();

            var dataResult = await _controller.Create(data);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("GetBlob")]
        public async Task<IActionResult> GetBlob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Blobs/Get/{id}")] HttpRequestMessage req, string id,
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

            var dataResult = await _controller.Get(id);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("UpdateBlob")]
        public async Task<IActionResult> UpdateBlob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Blobs/Update")] HttpRequestMessage req,
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

            BlobModel data = await req.Content.ReadAsAsync<BlobModel>();

            var dataResult = await _controller.Update(data);

            if (!dataResult.Success)
            {
                return new BadRequestObjectResult(dataResult.Message);
            }

            return new OkObjectResult(dataResult.Value);
        }

        [FunctionName("GetAll")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blobs/GetAll")] HttpRequestMessage req,
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

        [FunctionName("DeleteBlob")]
        public async Task<IActionResult> DeleteBlob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Blobs/Delete/{id}")] HttpRequestMessage req, string id,
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


        [FunctionName("GetSASToken")]
        public async Task<IActionResult> GetSASToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Blobs/GetSASToken")] HttpRequestMessage req,
            HttpRequest request)
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

            var blobName = req.RequestUri.ParseQueryString().Get("blobName");
            var permissionsStorage = req.RequestUri.ParseQueryString().Get("permissions");
            var sizeQuery = req.RequestUri.ParseQueryString().Get("size");
            var limit = _settings.LimitBytesSizeContainerBlobs;

            if (blobName == null || blobName.Trim() == String.Empty)
            {
                return new BadRequestObjectResult("Specify value for blob guid");
            }

            var permissions = SharedAccessBlobPermissions.Read;
            bool success = Enum.TryParse(permissionsStorage.ToString(), out permissions);

            if (!success)
            {
                return new BadRequestObjectResult("Invalid value for permissions");
            }

            var size = sizeQuery != null ? Convert.ToInt32(sizeQuery) : 0;

            if (size == 0 || limit == 0)
            {
                return new BadRequestObjectResult("Upgrade");
            }

            Result<TokenDtoOutput> result = await _controller.GenerateToken(permissionsStorage, size, limit, permissions, blobName);

            if (!result.Success)
            {
                return new BadRequestObjectResult(result.Message);
            }

            return new OkObjectResult(result.Value);
        }

        [FunctionName("WarmFunctions")]
        public static void Run([TimerTrigger("0 */4 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private Result<ClaimsPrincipal> validAuthorized(HttpRequestMessage req, HttpRequest request)
        {
            return _userInfo.ValidateTokenAsync(req.Headers, request.HttpContext.Connection.RemoteIpAddress);
        }
    }
}
