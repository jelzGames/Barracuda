using Bases.Services;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Users.Domain.Interfaces;
using Users.Domain.Models;

namespace Users.Domain.Services
{
    public class UsersServices : IUsersServices

    {
        private readonly IUsersRepository _repository;
        public UsersServices(
            IUsersRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<Result<string>> Create(UserModel item)
        {
            return await _repository.Create(item);
        }

        public async Task<Result<string>> Delete(string id)
        {
            return await _repository.Delete(id);
        }

        public async Task<Result<UserModel>> Get(string id)
        {
            return await _repository.Get(id);
        }

        public async Task<Result<UserQueryModel>> GetAll(QueryInputModel query, CancellationToken token)
        {
            return await _repository.GetAll(query, token);
        }

        public async Task<Result<string>> Update(UserModel item)
        {
            return await _repository.Update(item);
        }
    }
}
