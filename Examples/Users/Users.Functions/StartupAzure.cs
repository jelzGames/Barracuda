//using AuthOpenId.Interfaces;
//using AuthOpenId.Services;
//using Bases.Interfaces;
//using Bases.Services;
//using CosmosDatabase.Interfaces;
//using CosmosDatabase.Services;
//using Microsoft.Azure.Functions.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using Users.Application.Interfaces;
//using Users.Application.Services;
//using Users.Domain.Interfaces;
//using Users.Domain.Services;
//using Users.Functions;
//using Users.Infrastructure;

//[assembly: FunctionsStartup(typeof(StartupAzure))]
//namespace Users.Functions
//{
//    public class StartupAzure : FunctionsStartup
//    {
//        public override void Configure(IFunctionsHostBuilder builder)
//        {
//            builder.Services.AddHttpClient();

//            string Endpoint = System.Environment.GetEnvironmentVariable("DatabaseEndpoint", EnvironmentVariableTarget.Process);
//            string SecretKey = System.Environment.GetEnvironmentVariable("DatabaseSecretKey", EnvironmentVariableTarget.Process);
//            string DatabaseId = System.Environment.GetEnvironmentVariable("DatabaseId", EnvironmentVariableTarget.Process);
//            int MaxItemCount = Convert.ToInt32(System.Environment.GetEnvironmentVariable("MaxItemCount", EnvironmentVariableTarget.Process));
            
//            builder.Services.AddSingleton<ICosmosDB>(
//                db => new CosmosDB(Endpoint, SecretKey, DatabaseId, MaxItemCount)
//            );

//            builder.Services.AddScoped<IUserApplication, UserApplication>();
//            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
//            builder.Services.AddScoped<IUsersServices, UsersServices>();
//            builder.Services.AddScoped<IUserInfo, OpenIdUserInfo>();
//            builder.Services.AddScoped<ITokens, Tokens>();
//            builder.Services.AddScoped<IResult, Result>();
//            builder.Services.AddScoped<IErrorMessages, ErrorMessages>();
//            builder.Services.AddScoped<ISettingsTokens, SettingsTokens>();

//            var scopeFactory = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

//            using (var serviceScope = scopeFactory.CreateScope())
//            {
//                var context = serviceScope.ServiceProvider.GetService<ICosmosDB>();
//                context.CreateDatabase();
//            }
//        }
//    }
//}
