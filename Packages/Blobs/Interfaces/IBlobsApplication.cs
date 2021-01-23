using Bases.Services;
using Blobs.Application.Dtos;
using Blobs.Domain.Models;
using Common.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blobs.Application.Interfaces
{
    public interface IBlobsApplication
    {
        public Task<Result<QueryAllModel<BlobModel>>> GetAll(QueryInputModel query, CancellationToken token);
        public Task<Result<BlobModel>> Get(string id);
        public Task<Result<string>> Create(BlobModel item);
        public Task<Result<string>> Update(BlobModel item);
        public Task<Result<string>> Delete(string id);
        public Task<Result<TokenDtoOutput>> GenerateToken(string permissionsStorage, int size, int limit,
           SharedAccessBlobPermissions permissions, string blobName);
    }
}
