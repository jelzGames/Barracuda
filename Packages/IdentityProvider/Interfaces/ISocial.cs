using Barracuda.Indentity.Provider.Models;
using Barracuda.Indentity.Provider.Services;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ISocial
    {
        public Task<Result<SocialModel>> GoogleValidateToken(dynamic data);
        public Task<Result<SocialModel>> FacebookValidateToken(dynamic data);
        public Task<Result<SocialModel>> MicrosoftValidateToken(dynamic data);
    }
}
