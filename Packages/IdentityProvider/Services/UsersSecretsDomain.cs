using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Interfaces;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Services
{
    public class UsersSecretsDomain : IUsersSecretsDomain
    {
        private readonly IUsersSecretsRepository _repository;
        public UsersSecretsDomain(
            IUsersSecretsRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<Result<string>> ChangePassword(string email, string password)
        {
            return await _repository.ChangePassword(email, password);
        }

        public async Task<Result<string>> CheckEmail(string email)
        {
            return await _repository.CheckEmail(email);
        }

        public async Task<Result<string>> DeleteUser(string id)
        {
            return await _repository.DeleteUser(id);
        }

        public async Task<Result<UserPrivateDataModel>> GetSecrets(string id)
        {
            return await _repository.GetSecrets(id);
        }

        public async Task<Result<UserPrivateDataModel>> Login(string email)
        {
            return await _repository.Login(email);
        }

        public async Task<Result<string>> Register(string email, string password, bool validEmail = false)
        {
            return await _repository.Register(email, password, validEmail);
        }
       
        public async Task<Result<string>> Update(UserPrivateDataModel model)
        {
            return await _repository.Update(model);
        }

        public async Task<Result<string>> ValidateRegisterEmail(string email)
        {
            return await _repository.ValidateRegisterEmail(email);
        }
    }
}
