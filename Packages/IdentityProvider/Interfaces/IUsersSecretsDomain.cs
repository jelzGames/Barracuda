﻿using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Services;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface IUsersSecretsDomain
    {
        Task<Result<string>> Register(string email, string password, bool validEmail = false);
        Task<Result<UserPrivateDataModel>> Login(string email);
        Task<Result<UserPrivateDataModel>> GetSecrets(string id);
        Task<Result<string>> Update(UserPrivateDataModel model);
        Task<Result<string>> ChangePassword(string email, string password);
        Task<Result<string>> ValidateRegisterEmail(string email);
    }
}
