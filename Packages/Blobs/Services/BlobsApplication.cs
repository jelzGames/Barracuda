using Bases.Services;
using Blobs.Application.Dtos;
using Blobs.Application.Interfaces;
using Blobs.Domain.Interfaces;
using Blobs.Domain.Models;
using Common.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blobs.Application.Services
{
    public class BlobsApplication : IBlobsApplication
    {
        private readonly IBlobsDomain _services;
        private readonly IGenerateBlobToken _token;
        
        public BlobsApplication(
            IBlobsDomain services,
            IGenerateBlobToken token
        )
        {
            _services = services;
            _token = token;
        }

        public async Task<Result<string>> Create(BlobModel item)
        {
            return await _services.Create(item);
        }

        public async Task<Result<string>> Delete(string id)
        {
            return await _services.Delete(id);
        }

        public async Task<Result<BlobModel>> Get(string id)
        {
            return await _services.Get(id);
        }

        public async Task<Result<QueryAllModel<BlobModel>>> GetAll(QueryInputModel query, CancellationToken token)
        {
            return await _services.GetAll(query, token);
        }

        public async Task<Result<string>> Update(BlobModel item)
        {
            return await _services.Update(item);
        }

        public async Task<Result<TokenDtoOutput>> GenerateToken(string permissionsStorage, int size, int limit,
            SharedAccessBlobPermissions permissions, string blobName)
        {
            return await _token.GenerateToken(permissionsStorage, size, limit, permissions, blobName);

        }
    }
}
