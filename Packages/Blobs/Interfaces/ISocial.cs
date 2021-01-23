using AuthOpenId.Models;
using Bases.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthOpenId.Interfaces
{
    public interface ISocial
    {
        public Task<Result<SocialModel>> GoogleValidateToken(dynamic data);
        public Task<Result<SocialModel>> FacebookValidateToken(dynamic data);
        public Task<Result<SocialModel>> MicrosoftValidateToken(dynamic data);
    }
}
