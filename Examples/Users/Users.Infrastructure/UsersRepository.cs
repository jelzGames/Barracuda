
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Users.Domain.Interfaces;
using Common;
using Common.Models;
using System.Threading.Tasks;
using System.Threading;
using CosmosDatabase.Interfaces;
using Bases.Services;
using Bases.Interfaces;
using Users.Domain.Models;
using Microsoft.Azure.Cosmos;

namespace Users.Infrastructure
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _partitionId = "users";
        private readonly ICosmosDBExample _contextCosmosDB;
        private readonly IResultExample _result;

        public UsersRepository(
            ICosmosDBExample contextCosmosDB,
            IResultExample result
        )
        {
            _contextCosmosDB = contextCosmosDB;
            _result = result;
        }
        
        private Container RepositoryContainer
        {
            get
            {
                return _contextCosmosDB.Client.GetContainer(_contextCosmosDB.DatabaseId, _contextCosmosDB.CollectionId);
            }
        }

        public async Task<Result<string>> CheckUsername(string username)
        {
            bool ok = false;
            string message = "";
            var find = false;

            try
            {
                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                var query = $"select * from d where d.Username = '{username}'";

                using (FeedIterator<UserModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserModel>(
                   query,
                   null,
                   queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var item in await feedIterator.ReadNextAsync())
                        {
                            find = true;
                            break;
                        }
                    }
                }

                if (find)
                {
                    message = "Found";
                }
                else
                {
                    ok = true;
                }
            }
            catch (CosmosException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return _result.Create(ok, message, "");
        }

        public async Task<Result<string>> Create(UserModel item)
        {
            bool ok = false;
            string message = "";

            try
            {
                if (String.IsNullOrEmpty(item.id))
                {
                    item.id = Guid.NewGuid().ToString();
                }
                UpdateMetadata(item, true);
                var tenatId = new PartitionKey(_partitionId);
                await RepositoryContainer.CreateItemAsync<UserModel>(item, tenatId);

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
                var tenatId = new PartitionKey(_partitionId);
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
        public async Task<Result<UserModel>> Get(string id)
        {
            bool ok = false;
            string message = "";
            UserModel model = null;

            try
            {
                var key = new PartitionKey(_partitionId);
                model = await RepositoryContainer.ReadItemAsync<UserModel>(id, key);

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
        public async Task<Result<UserQueryModel>> GetAll(QueryInputModel query, CancellationToken token)
        {
            bool ok = false;
            string message = "";
            List<UserModel> models = new List<UserModel>();
            UserQueryModel model = new UserQueryModel();
            string newContinuationToken = null;

            try
            {
                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = _contextCosmosDB.MaxItemCount
                };

                using (FeedIterator<UserModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserModel>(
                   query.Query,
                   query.ContinuationToken,
                   queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        FeedResponse<UserModel> response = await feedIterator.ReadNextAsync();
                        newContinuationToken = response.ContinuationToken;
                        foreach (var item in response.Resource)
                        {
                            models.Add(item);
                        }
                    }
                }

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
        public async Task<Result<string>> Update(UserModel item)
        {
            bool ok = false;
            string message = "";

            try
            {
                UpdateMetadata(item, false);
                var tenatId = new PartitionKey(_partitionId);
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
        private void UpdateMetadata(UserModel item, bool isAdded)
        {
            if (isAdded)
            {
                //item.CreatedBy = _userInfo?.UserId;
                item.TimeCreated = DateTime.Now;
            }
            else
            {
                //item.ModifiedBy = _userInfo?.UserId;
                item.TimeModified = DateTime.Now;
            }

            item.PartitionId = _partitionId;
        }
    }
}
