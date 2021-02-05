using Bases.Services;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.Interfaces;
using Users.Domain.Interfaces;
using Users.Domain.Models;

namespace Users.Application.Services
{
    public class UserApplication : IUserApplication

    {
        private readonly IUsersServices _services;
        public UserApplication(
            IUsersServices services
        )
        {
            _services = services;
        }

        public async Task<Result<string>> Create(UserModel item)
        {
            return await _services.Create(item);
        }

        public async Task<Result<string>> Delete(string id)
        {
            return await _services.Delete(id);
        }

        public async Task<Result<UserModel>> Get(string id)
        {
            return await _services.Get(id);
        }

        public async Task<Result<UserQueryModel>> GetAll(QueryInputModel query, CancellationToken token)
        {
            return await _services.GetAll(query, token);
        }

        public async Task<Result<string>> Update(UserModel item)
        {
            return await _services.Update(item);
        }
        public async Task<Result<string>> CheckUsername(string username)
        {
            return await _services.CheckUsername(username);
        }
    }
}
