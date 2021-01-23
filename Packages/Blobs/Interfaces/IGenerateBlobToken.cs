using Bases.Services;
using Blobs.Application.Dtos;
using Common.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blobs.Application.Interfaces
{
    public interface IGenerateBlobToken
    {
        public Task<Result<TokenDtoOutput>> GenerateToken(string permissionsStorage, int size, int limit,
           SharedAccessBlobPermissions permissions, string blobName);
    }
}
