using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Models;
using Barracuda.Indentity.Provider.Services;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface IUsersSecretsRepository
    {
        Task<Result<string>> Register(string userid, string email, string password, bool validEmail = false);
        Task<Result<string>> DeleteUser(string id);
        Task<Result<UserPrivateDataModel>> Login(string email);
        Task<Result<UserPrivateDataModel>> GetSecrets(string id);
        Task<Result<string>> Update(UserPrivateDataModel model);
        Task<Result<string>> ChangePassword(string email, string password);
        Task<Result<string>> ValidateRegisterEmail(string email);
        Task<Result<string>> CheckEmail(string email);
        Task<Result<AdditionalModel>> GetAdditional(string id);
    }
}
