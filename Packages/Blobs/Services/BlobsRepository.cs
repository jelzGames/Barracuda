using AuthOpenId.Interfaces;
using Bases.Interfaces;
using Bases.Services;
using Blobs.Domain.Interfaces;
using Blobs.Domain.Models;
using Common.Models;
using CosmosDatabase.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blobs.Infrastructure
{
    public class BlobsRepository : IBlobsRepository
    {
        private readonly string _partitionId = "blobs";
        private readonly ICosmosDB _contextCosmosDB;
        private readonly IUserInfo _userInfo;
        private readonly IResult _result;

        public BlobsRepository(
            ICosmosDB contextCosmosDB,
            IUserInfo userInfo,
            IResult result
        )
        {
            _contextCosmosDB = contextCosmosDB;
            _userInfo = userInfo;
            _result = result;
        }

        private Container RepositoryContainer
        {
            get
            {
                return _contextCosmosDB.Client.GetContainer(_contextCosmosDB.DatabaseId, _contextCosmosDB.CollectionId);
            }
        }

        public async Task<Result<string>> Create(BlobModel item)
        {
            bool ok = false;
            string message = "";

            try
            {
                item.id = Guid.NewGuid().ToString();
                UpdateMetadata(item, true);
                var tenatId = new PartitionKey(_partitionId + "_" + _userInfo.UserId);
                await RepositoryContainer.CreateItemAsync<BlobModel>(item, tenatId);

                message = item.id;

                ok = true;
            }
            catch (CosmosException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return _result.Create<string>(ok, message, message);
        }

        public async Task<Result<string>> Delete(string id)
        {
            bool ok = false;
            string message = "";
            try
            {
                var tenatId = new PartitionKey(_partitionId + "_" + _userInfo.UserId);
                await RepositoryContainer.DeleteItemAsync<string>(id, tenatId);

                ok = true;
            }
            catch (CosmosException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return _result.Create<string>(ok, message, "");
        }

        public async Task<Result<BlobModel>> Get(string id)
        {
            bool ok = false;
            string message = "";
            BlobModel model = null;

            try
            {
                var key = new PartitionKey(_partitionId + "_" + _userInfo.UserId);
                model = await RepositoryContainer.ReadItemAsync<BlobModel>(id, key);

                ok = true;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    message = Enum.GetName(typeof(System.Net.HttpStatusCode), System.Net.HttpStatusCode.NotFound);
                }
                else
                {
                    message = e.Message;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return _result.Create(ok, message, model);

        }

        public async Task<Result<QueryAllModel<BlobModel>>> GetAll(QueryInputModel query, CancellationToken token)
        {
            bool ok = false;
            string message = "";
            IEnumerable<BlobModel> models = null;
            QueryAllModel<BlobModel> model = new QueryAllModel<BlobModel>();
            string newContinuationToken = null;

            try
            {
                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId + "_" + _userInfo.UserId),
                    MaxItemCount = _contextCosmosDB.MaxItemCount
                };

                //await foreach (var page in RepositoryContainer.GetItemQueryIterator<BlobModel>(query.Query, query.ContinuationToken, queryOptions, token).AsPages())
                //{
                //    models = page.Values;
                //    newContinuationToken = page.ContinuationToken;
                //    break;
                //}

                model.ContinuationToken = newContinuationToken;
                model.models = models;
                ok = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return _result.Create(ok, message, model);
        }

        public async Task<Result<string>> Update(BlobModel item)
        {
            bool ok = false;
            string message = "";

            try
            {
                UpdateMetadata(item, false);
                var tenatId = new PartitionKey(_partitionId + "_" + _userInfo.UserId);
                await RepositoryContainer.ReplaceItemAsync<dynamic>(item, item.id, tenatId,
                    new ItemRequestOptions { });

                ok = true;
            }
            catch (CosmosException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return _result.Create<string>(ok, message, "");
        }

        private void UpdateMetadata(BlobModel item, bool isAdded)
        {
            item.Container = _userInfo.UserId;

            if (isAdded)
            {
                item.CreatedBy = _userInfo?.UserId;
                item.TimeCreated = DateTime.Now;
            }
            else
            {
                item.ModifiedBy = _userInfo?.UserId;
                item.TimeModified = DateTime.Now;
            }


            if (string.IsNullOrEmpty(item.PartitionId))
            {
                item.PartitionId = _partitionId + "_" + _userInfo.UserId;
            }
        }
    }
}
