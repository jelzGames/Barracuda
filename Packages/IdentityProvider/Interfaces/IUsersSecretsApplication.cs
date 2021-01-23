using Barracuda.Indentity.Provider.Dtos;
using Barracuda.Indentity.Provider.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface IUsersSecretsApplication
    {
        Task<Result<string>> Register(string email, string password);
        Task<Result<string>> UpdateScopes(string id, dynamic scopes);
        Task<Result<LoginDto>> Login(string email, string password, HttpRequest request);
        Task<Result<LoginDto>> Refresh(string token, string refreshToken, HttpRequest request);
        Task<Result<LoginDto>> RefreshToken(string id, string email, HttpRequest request);
        Task<Result<LoginDto>> GoogleValidateToken(dynamic data, HttpRequest request);
        Task<Result<LoginDto>> FacebookValidateToken(dynamic data, HttpRequest request);
        Task<Result<LoginDto>> MicrosoftValidateToken(dynamic data, HttpRequest request);
        LoginDto EndToken(HttpRequest request, string cookie, string path, UserPrivateDataDto model);
        TokensDto ValidateToken(HttpRequestMessage req);
        string ValidateToken(string token);
        Task<Result<string>> ChangePassword(string email, string password);
        string ForgotPassword(string email);
    }
}
