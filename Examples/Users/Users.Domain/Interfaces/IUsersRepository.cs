﻿using Bases.Services;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Users.Domain.Models;

namespace Users.Domain.Interfaces
{
    public interface IUsersRepository
    {
        public Task<Result<UserQueryModel>> GetAll(QueryInputModel query, CancellationToken token);
        public Task<Result<UserModel>> Get(string id);
        public Task<Result<string>> Create(UserModel item);
        public Task<Result<string>> Update(UserModel item);
        public Task<Result<string>> Delete(string id);
        Task<Result<string>> CheckUsername(string username);
    }
}
