using Bases.Services;
using Blobs.Domain.Interfaces;
using Blobs.Domain.Models;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blobs.Domain.Services
{
    public class BlobsDomain : IBlobsDomain
    {
        private readonly IBlobsRepository _repository;
        public BlobsDomain(
            IBlobsRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<Result<string>> Create(BlobModel item)
        {
            return await _repository.Create(item); 
        }

        public async Task<Result<string>> Delete(string id)
        {
            return await _repository.Delete(id);
        }

        public async Task<Result<BlobModel>> Get(string id)
        {
            return await _repository.Get(id);
        }

        public async Task<Result<QueryAllModel<BlobModel>>> GetAll(QueryInputModel query, CancellationToken token)
        {
            return await _repository.GetAll(query, token);
        }

        public async Task<Result<string>> Update(BlobModel item)
        {
            return await _repository.Update(item);
        }
    }
}
