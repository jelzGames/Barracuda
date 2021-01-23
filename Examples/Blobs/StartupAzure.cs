using AuthOpenId.Interfaces;
using AuthOpenId.Services;
using Bases.Interfaces;
using Bases.Services;
using Blobs.Application.Interfaces;
using Blobs.Application.Services;
using Blobs.Domain.Interfaces;
using Blobs.Domain.Services;
using Blobs.Functions;
using Blobs.Infrastructure;
using CosmosDatabase.Interfaces;
using CosmosDatabase.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(StartupAzure))]
namespace Blobs.Functions
{
    public class StartupAzure : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            string Endpoint = System.Environment.GetEnvironmentVariable("DatabaseEndpoint", EnvironmentVariableTarget.Process);
            string SecretKey = System.Environment.GetEnvironmentVariable("DatabaseSecretKey", EnvironmentVariableTarget.Process);
            string DatabaseId = System.Environment.GetEnvironmentVariable("DatabaseId", EnvironmentVariableTarget.Process);
            int MaxItemCount = Convert.ToInt32(System.Environment.GetEnvironmentVariable("MaxItemCount", EnvironmentVariableTarget.Process));

            builder.Services.AddSingleton<ICosmosDB>(
                db => new CosmosDB(Endpoint, SecretKey, DatabaseId, MaxItemCount)
            );

            builder.Services.AddScoped<IBlobsDomain, BlobsDomain>();
            builder.Services.AddScoped<IBlobsApplication, BlobsApplication>();
            builder.Services.AddScoped<IBlobsRepository, BlobsRepository>();
            builder.Services.AddScoped<IGenerateBlobToken, GenerateBlobToken>();
            builder.Services.AddScoped<IUserInfo, OpenIdUserInfo>();
            builder.Services.AddScoped<ITokens, Tokens>();
            builder.Services.AddScoped<IResult, Result>();
            builder.Services.AddScoped<IErrorMessages, ErrorMessages>();
            builder.Services.AddScoped<ISettingsTokens, SettingsTokens>();
            builder.Services.AddScoped<ISettingsBlobs, SettingsBlobs>();

            var scopeFactory = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

            using (var serviceScope = scopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ICosmosDB>();
                context.CreateDatabase();
            }
        }
    }
}
