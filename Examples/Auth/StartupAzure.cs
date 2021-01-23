using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using UsersSecrets.Functions;
using Barracuda.Indentity.Provider.Interfaces;
using Barracuda.Indentity.Provider.Services;

[assembly: FunctionsStartup(typeof(StartupAzure))]
namespace UsersSecrets.Functions
{
    public class StartupAzure : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
         
            builder.Services.AddScoped<IUsersSecretsApplication, UsersSecretsApplication>();
            builder.Services.AddScoped<IUsersSecretsRepository, UsersSecretsRepository>();
            builder.Services.AddScoped<IUsersSecretsDomain, UsersSecretsDomain>();
            builder.Services.AddScoped<IUserInfo, OpenIdUserInfo>();
            builder.Services.AddScoped<ISettingsUserSecrests, SettingsUserSecrets>();
            builder.Services.AddScoped<ITokens, Tokens>();
            builder.Services.AddScoped<ISocial, Social>();
            builder.Services.AddScoped<IRedisCache, RedisCache>();
            builder.Services.AddScoped<ICryptograhic, Cryptograhic>();
            builder.Services.AddScoped<IResult, Result>();
            builder.Services.AddScoped<IErrorMessages, ErrorMessages>();
            builder.Services.AddScoped<ISettingsRedis, SettingsRedis>();
            builder.Services.AddScoped<ISettingsTokens, SettingsTokens>();
            builder.Services.AddScoped<ISettingsCosmos, SettingsCosmos>();
            builder.Services.AddScoped<ICosmosDB, CosmosDB>();
          
            var scopeFactory = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

            using (var serviceScope = scopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ICosmosDB>();
                context.CreateDatabase();
            }
        }
    }
}
