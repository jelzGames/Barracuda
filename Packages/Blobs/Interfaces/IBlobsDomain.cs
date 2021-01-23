using Bases.Services;
using Blobs.Domain.Models;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blobs.Domain.Interfaces
{
    public interface IBlobsDomain
    {
        public Task<Result<QueryAllModel<BlobModel>>> GetAll(QueryInputModel query, CancellationToken token);
        public Task<Result<BlobModel>> Get(string id);
        public Task<Result<string>> Create(BlobModel item);
        public Task<Result<string>> Update(BlobModel item);
        public Task<Result<string>> Delete(string id);
    }
}
