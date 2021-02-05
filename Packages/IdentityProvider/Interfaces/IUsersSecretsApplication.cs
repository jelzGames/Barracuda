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
        Task<Result<string>> Register(string email, string password, bool validEmail = false);
        Task<Result<string>> DeleteUser(string id);
        Task<Result<string>> UpdateScopes(string id, List<string> scopes);
        Task<Result<string>> UpdateTenants(string id, List<string> tenants);
        Task<Result<LoginDto>> Login(string email, string password, HttpRequest request);
        Task<Result<LoginDto>> Refresh(string token, string refreshToken, HttpRequest request);
        Task<Result<LoginDto>> RefreshToken(string id, string email, HttpRequest request);
        Task<Result<LoginDto>> GoogleValidateToken(dynamic data, HttpRequest request);
        Task<Result<LoginDto>> FacebookValidateToken(dynamic data, HttpRequest request);
        Task<Result<LoginDto>> MicrosoftValidateToken(dynamic data, HttpRequest request);
        LoginDto EndToken(HttpRequest request, string cookie, string path, UserPrivateDataDto model);
        TokensDto ValidateToken(HttpRequestMessage req);
        string ValidateToken(string token);
        string ValidateTokenConfirmEmail(string token);
        Task<Result<string>> ChangePassword(string email, string password);
        string ForgotPasswordOrRegister(string email);
        Task<Result<string>> ValidateRegisterEmail(string email);
        Task<Result<string>> CheckEmail(string email);
    }
}
