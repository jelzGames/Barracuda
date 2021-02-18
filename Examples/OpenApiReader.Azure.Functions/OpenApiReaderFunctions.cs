using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Barracuda.OpenApi.Interfaces;
using Barracuda.OpenApi.Attributes;
using Demo.Azure.Functions.Models;
using System.Net;
using Barracuda.Indentity.Provider.Services;
using System.Security.Claims;
using Barracuda.Indentity.Provider.Interfaces;

namespace Demo.Azure.Functions
{
    [BOAVersion("3.0.1")]
    [BOAInfo("Demo API", "v1")]

    public class OpenApiReaderFunctions
    {
        public readonly IOpenApiBuilder _builder;
        private readonly IResult _result;
        private readonly IUserInfo _userInfo;

        public OpenApiReaderFunctions(
            IOpenApiBuilder builder,
            IUserInfo userInfo,
            IResult result
            )
        {
            _builder = builder;
            _userInfo = userInfo;
            _result = result;
        }

        [FunctionName("GetAll")]
        
        /*
           open api reader attributes 
        */
        [BOA]
        [BOAFunctionName("GetAll")]
        [BOASummary("Get products register")]
        [BOAParameter("up-tenant-id", "header", "Tenant id", typeof(string), Required = true)]
        [BOAParameter("filter", "query", "Filter", typeof(string))]
        [BOAParameter("sort", "query", "Sort", typeof(string))]
        [BOAParameter("page", "query", "Page", typeof(int))]
        [BOAParameter("productGroup", "query", "Product Group", typeof(string))]
        [BOAParameter("productUnit", "query", "Product Unit", typeof(string))]
        [BOAProducesResponse(typeof(List<DemoModels>))]
        [BOARoute("/api/openApiDemos/GetAll")]
        [BOAHttpGet]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "openApiDemos/GetAll")] HttpRequestMessage req,
            HttpRequest request, ILogger log)
        {
            var resultAuth = validAdmin(req, request, new List<string>() { "users.read" });
            if (!resultAuth.Success)
            {
                return new BadRequestObjectResult(resultAuth.Message);
            }

            return await Task.FromResult(new OkObjectResult(new List<DemoModels>()));
        }

        [FunctionName("GetWithId")]
      
        [BOA]
        [BOAFunctionName("GetWithId")]
        [BOASummary("Get a product register")]
        [BOAParameter("up-tenant-id", "header", "Tenant id", typeof(string), Required = true)]
        [BOAParameter("id", "path", "Item Code", typeof(string), Required = true)]
        [BOAProducesResponse(typeof(DemoModels))]
        [BOARoute("/api/openApiDemos/Get/{id}")]
        [BOAHttpGet]
        public async Task<IActionResult> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openApiDemos/Get/{id}")] HttpRequestMessage req, 
           string id, HttpRequest request)
        {
            return await Task.FromResult(new OkObjectResult(new DemoModels()));
        }

        [FunctionName("Post")]
       
        [BOA]
        [BOAFunctionName("GetWithId")]
        [BOASummary("Add, update or delete a product register")]
        [BOAParameter("up-tenant-id", "header", "Tenant id", typeof(string), Required = true)]
        [BOARequestBody(typeof(DemoModels), "Product register")]
        [BOAProducesResponse(typeof(String))]
        [BOARoute("/api/openApiDemos/POST")]
        [BOAHttpPost]
        public async Task<IActionResult> Post(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "openApiDemos/POST")] HttpRequestMessage req,
         HttpRequest request)
        {

            DemoModels data = await req.Content.ReadAsAsync<DemoModels>();

            return await Task.FromResult(new OkObjectResult("demo post"));
        }

        [FunctionName("OpenAPI")]
        public async Task<IActionResult> OpenAPI([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openApiDemos/openapi/v1")] HttpRequestMessage request)
        {
            return new OkObjectResult(await Task.FromResult(
                _builder.Build(Assembly.GetExecutingAssembly(), this.GetType().Name, request)
                ));
        }


        [FunctionName("OpenAPIUI")]
        public async Task<IActionResult> OpenAPIUI([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openApiDemos/openapi/ui")] HttpRequestMessage request)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = await Task.FromResult(_builder.OpenAPIUI())
            };
        }

        [FunctionName("OpenAPIAuth")]
        public async Task<IActionResult> OpenAPIAuth([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openApiDemos/openapi/auth")] HttpRequestMessage request)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = await Task.FromResult(_builder.OpenAPIAuth())
            };
        }
        private Result<bool> validAdmin(HttpRequestMessage req, HttpRequest request, List<string> scopes = null, List<string> tenants = null)
        {
            var resultAuth = validAuthorized(req, request, scopes, tenants);
            if (!resultAuth.Success)
            {
                return _result.Create<bool>(false, resultAuth.Message, false);
            }

            return _result.Create<bool>(true, "", true);
        }
        private Result<ClaimsPrincipal> validAuthorized(HttpRequestMessage req, HttpRequest request, List<string> scopes = null, List<string> tenants = null)
        {
            return _userInfo.ValidateTokenAsync(req.Headers, request.HttpContext.Connection.RemoteIpAddress, scopes, tenants);
        }
    }
}
