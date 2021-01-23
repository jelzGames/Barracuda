using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Barracuda.OpenApi.Attributes;
using Barracuda.OpenApi.Interfaces;
using Barracuda.OpenApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OpenApiLambdaAws
{
    public class Functions
    {
        public IOpenApiBuilder _builder { get; }
   
        public Functions()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _builder = serviceProvider.GetService<IOpenApiBuilder>();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IOpenApiBuilder, OpenApiBuilder>();
            serviceCollection.AddScoped<IOpenApiReader, OpenApiReader>();
            serviceCollection.AddScoped<ISettingsOpenApi, SettingsOpenApi>();
        }


        [BOA]
        [BOAFunctionName("GetAll")]
        [BOASummary("Get products register")]
        [BOAParameter("up-tenant-id", "header", "Tenant id", typeof(string), Required = true)]
        [BOAParameter("filter", "query", "Filter", typeof(string))]
        [BOAParameter("sort", "query", "Sort", typeof(string))]
        [BOAParameter("page", "query", "Page", typeof(int))]
        [BOAParameter("productGroup", "query", "Product Group", typeof(string))]
        [BOAParameter("productUnit", "query", "Product Unit", typeof(string))]
        [BOAProducesResponse(typeof(List<DemoModel>))]
        [BOARoute("/api/openApiDemos/GetAll")]
        [BOAHttpGet]
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAll(string input, ILambdaContext context)
        {
            return await Task.FromResult(new OkObjectResult(new List<DemoModel>()));
        }

        public async Task<IActionResult> OpenAPI(string input, ILambdaContext context)
        {
            return new OkObjectResult(await Task.FromResult(
                _builder.Build(Assembly.GetExecutingAssembly(), this.GetType().Name, "OpenApiLambdaAws::OpenApiLambdaAws.Functions::OpenAPI")
                ));
        }

        public async Task<IActionResult> OpenAPIUI(string input, ILambdaContext context)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = await Task.FromResult(_builder.OpenAPIUI())
            };
        }

        public async Task<IActionResult> OpenAPIAuth(string input, ILambdaContext context)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = await Task.FromResult(_builder.OpenAPIAuth())
            };
        }
    }
}
