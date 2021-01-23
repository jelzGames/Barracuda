using Bases.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthOpenId.Interfaces
{
    public interface IUserInfo
    {
        string UserId { get; }
        string Email { get; }
        Task<string> GetTokenAsync();
        ICollection<string> Groups { get; }

        Result<ClaimsPrincipal> ValidateTokenAsync(HttpRequestHeaders value, IPAddress ipAddress);
    }
}
