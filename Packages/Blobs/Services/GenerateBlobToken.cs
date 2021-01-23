using AuthOpenId.Interfaces;
using Bases.Interfaces;
using Bases.Services;
using Blobs.Application.Dtos;
using Blobs.Application.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blobs.Application.Services
{
    public class GenerateBlobToken : IGenerateBlobToken
    {
        private readonly ISettingsBlobs _settings;
        private readonly IUserInfo _userInfo;
        private readonly IResult _result;

        public GenerateBlobToken(
            ISettingsBlobs settings,
            IUserInfo userInfo,
            IResult result
            )
        {
            _settings = settings;
            _userInfo = userInfo;
            _result = result;
        }

        public async Task<Result<TokenDtoOutput>> GenerateToken(string permissionsStorage, int size, int limit, 
            SharedAccessBlobPermissions permissions, string blobName)
        {
            var storageAccount = CloudStorageAccount.Parse(_settings.StorageAccountConnection);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerName = _userInfo.UserId;
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            if (permissionsStorage == "Write")
            {
                long fileSize = size;
                BlobContinuationToken continuationToken = null;
                CloudBlob blob;
                var segmentSize = 100;
                do
                {
                    BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(string.Empty,
                        true, BlobListingDetails.Metadata, segmentSize, continuationToken, null, null);

                    foreach (var blobItem in resultSegment.Results)
                    {
                        blob = (CloudBlob)blobItem;

                        fileSize += blob.Properties.Length;
                        if (limit <= fileSize)
                        {
                            return _result.Create<TokenDtoOutput>(false, "Upgrade", null); 
                        }
                    }

                    continuationToken = resultSegment.ContinuationToken;

                } while (continuationToken != null);
            }

            var sasToken = GetBlobSasToken(container, blobName, permissions);

            /*
            var sasToken =  
                data.blobName != null ?
                    GetBlobSasToken(container, data.blobName.ToString(), permissions) :
                    GetContainerSasToken(container, permissions);
            */

            var uri = "";

            if (permissionsStorage == "Read")
            {
                uri = container.Uri + "/" + blobName + sasToken;

            }
            if (permissionsStorage == "Delete")
            {
                uri = storageAccount.BlobEndpoint.AbsoluteUri + sasToken;
            }
            if (permissionsStorage == "Write")
            {
                uri = storageAccount.BlobEndpoint.AbsoluteUri + sasToken;
            }

            var tokenModel = new TokenDtoOutput
            {
                Container = containerName,
                BlobName = blobName,
                Token = sasToken,
                Permission = permissionsStorage,
                Uri = uri
            };

            return _result.Create<TokenDtoOutput>(true, "", tokenModel);
        }

        private string GetBlobSasToken(CloudBlobContainer container, string blobName, SharedAccessBlobPermissions permissions, string policyName = null)
        {
            string sasBlobToken;

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            if (policyName == null)
            {
                var adHocSas = CreateAdHocSasPolicy(permissions);

                sasBlobToken = blob.GetSharedAccessSignature(adHocSas);
            }
            else
            {
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }

            return sasBlobToken;
        }

        private string GetContainerSasToken(CloudBlobContainer container, SharedAccessBlobPermissions permissions, string storedPolicyName = null)
        {
            string sasContainerToken;

            if (storedPolicyName == null)
            {
                var adHocSas = CreateAdHocSasPolicy(permissions);

                sasContainerToken = container.GetSharedAccessSignature(adHocSas, null);
            }
            else
            {
                sasContainerToken = container.GetSharedAccessSignature(null, storedPolicyName);
            }

            return sasContainerToken;
        }

        private SharedAccessBlobPolicy CreateAdHocSasPolicy(SharedAccessBlobPermissions permissions)
        {
            return new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(_settings.AddMinutesToStartTime),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(_settings.AddMinutesToEndTime),
                Permissions = permissions
            };
        }

        private string GenerateContainerName(string tenantid)
        {
            return ("up" + ComputeHash(_settings.BlobKey + tenantid)).Substring(0, 32);
        }

        private string ComputeHash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }

            return Sb.ToString();
        }

    }
}
