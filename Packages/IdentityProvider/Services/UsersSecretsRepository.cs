using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Interfaces;
using Barracuda.Indentity.Provider.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Services
{
    public class UsersSecretsRepository : IUsersSecretsRepository
    {
        private readonly string _partitionId = "userssecrets";
        private readonly ICosmosDB _contextCosmosDB;
        private readonly ISettingsUserSecrests _settings;
        private readonly ITokens _tokens;
        private readonly ICryptograhic _crypto;
        private readonly IResult _result;
        private readonly IErrorMessages _errors;

        public UsersSecretsRepository(
            ICosmosDB contextCosmosDB,
            ISettingsUserSecrests settings,
            ITokens tokens,
            ICryptograhic crypto,
            IResult result,
            IErrorMessages errors
        )
        {
            _contextCosmosDB = contextCosmosDB;
            _settings = settings;
            _tokens = tokens;
            _crypto = crypto;
            _result = result;
            _errors = errors;
        }

        private Container RepositoryContainer
        {
            get
            {
                return _contextCosmosDB.Client.GetContainer(_contextCosmosDB.DatabaseId, _contextCosmosDB.CollectionId);
            }
        }

        public async Task<Result<UserPrivateDataModel>> Login(string email)
        {
            bool ok = false;
            string message = "";
            UserPrivateDataModel model = null;

            try
            {
                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };
                
                var query = $"select * from d where d.Email = '{email}'";
                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query,
                    null,
                    queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var item in await feedIterator.ReadNextAsync())
                        {
                            model = item;
                            break;
                        }
                    }
                }

                if (model == null)
                {
                    message = _errors.NotFound;
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

            return _result.Create(ok, message, model);
        }

        public async Task<Result<string>> Register(string userid, string email, string password, bool validEmail = false)
        {
            bool ok = false;
            string message = "";
            string id = "";
            
            try
            {

                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                var isFound = false;
                var query = $"select d.id, d.Email from d where d.Email = '{email}'";
                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query,
                    null,
                    queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var item in await feedIterator.ReadNextAsync())
                        {
                            id = item.id;
                            isFound = true;
                            break;
                        }
                    }
                }

                if (isFound)
                {
                    message = _errors.Found;
                }
                else
                {
                    var item = new UserPrivateDataModel();

                    if (String.IsNullOrEmpty(userid))
                    {
                        item.id = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        item.id = userid;
                    }
                    UpdatePrivateMetadata(item, email, password, false, validEmail);
                    var key = new PartitionKey(_partitionId);
                    await RepositoryContainer.CreateItemAsync<UserPrivateDataModel>(item, key,
                        new ItemRequestOptions { });

                    id = item.id;

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

            return _result.Create(ok, message, id);
        }

        public async Task<Result<string>> DeleteUser(string id)
        {
            bool ok = false;
            string message = "";
            
            try
            {

                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                var key = new PartitionKey(_partitionId);
                await RepositoryContainer.DeleteItemAsync<UserPrivateDataModel>(id, key,
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

            return _result.Create(ok, message, id);
        }

        public async Task<Result<string>> ValidateRegisterEmail(string email)
        {
            bool ok = false;
            string message = "";
            UserPrivateDataModel item = null;

            try
            {

                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                var isFound = false;
                var query = $"select * from d where d.Email = '{email}'";

                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query,
                    null,
                    queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var itemBack in await feedIterator.ReadNextAsync())
                        {
                            item = itemBack;
                            isFound = true;
                            break;
                        }
                    }
                }

                if (!isFound)
                {
                    message = _errors.NotFound;
                }
                else
                {
                    item.ValidEmail = true;
                    UpdatePrivate(item);
                    var key = new PartitionKey(_partitionId);
                    await RepositoryContainer.ReplaceItemAsync<UserPrivateDataModel>(item, item.id, key,
                        new ItemRequestOptions { });

                    message = item.id;

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

            return _result.Create(ok, message, message);
        }
        public async Task<Result<string>> ChangePassword(string email, string password)
        {
            bool ok = false;
            string message = "";
            string id = "";

            try
            {

                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                UserPrivateDataModel model = null;
                var isFound = false;
                var query = $"select * from d where d.Email = '{email}'";
                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                   query,
                   null,
                   queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var itemBack in await feedIterator.ReadNextAsync())
                        {
                            model = itemBack;
                            isFound = true;
                            break;
                        }
                    }
                }

                if (!isFound)
                {
                    message = _errors.NotFound;
                }
                else
                {
                    UpdatePrivateMetadata(model, email, password, true);
                    var key = new PartitionKey(_partitionId);
                    await RepositoryContainer.ReplaceItemAsync(model, model.id, key);

                    id = model.id;

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

            return _result.Create(ok, message, id);
        }

        private void UpdatePrivateMetadata(UserPrivateDataModel item, string email, string password, bool isUpdate, bool validEmail = false)
        {
            if (!isUpdate)
            {       
                
                item.Email = email;
                item.TimeCreated = DateTime.Now;
                item.PartitionId = _partitionId;
                item.ValidEmail = validEmail;
            }
            else
            {
                item.TimeModified = DateTime.Now;
            }

            item.Password = _crypto.GetStringSha256Hash(email + password + _settings.SecretKey);
        }
        private void UpdatePrivate(UserPrivateDataModel item)
        {
            
            item.TimeModified = DateTime.Now;


        }
        public async Task<Result<UserPrivateDataModel>> GetSecrets(string id)
        {
            bool ok = false;
            string message = "";
            UserPrivateDataModel model = null;

            try
            {
                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                var isFound = false;
                var query = $"select * from d where d.id = '{id}'";

                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                   query,
                   null,
                   queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var itemBack in await feedIterator.ReadNextAsync())
                        {
                            model = itemBack;
                            isFound = true;
                            break;
                        }
                    }
                }

                if (!isFound)
                {
                    message = _errors.NotFound;
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

            return _result.Create(ok, message, model);
        }

        public async Task<Result<string>> Update(UserPrivateDataModel model)
        {
            bool ok = false;
            string message = "";
            
            try
            {
                model.TimeModified = DateTime.Now;
                var key = new PartitionKey(_partitionId);
                await RepositoryContainer.ReplaceItemAsync<UserPrivateDataModel>(model, model.id, key,
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

            return _result.Create(ok, message, "");
        }

        public async Task<Result<string>> CheckEmail(string email)
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

                var query = $"select * from d where d.Email = '{email}'";

                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                   query,
                   null,
                   queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var itemBack in await feedIterator.ReadNextAsync())
                        {
                            find = true;
                            break;
                        }
                    }
                }
                
                if (find)
                {
                    message = _errors.Found;
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

        public async Task<Result<AdditionalModel>> GetAdditional(string id)
        {
            bool ok = false;
            string message = "";
            AdditionalModel result = new AdditionalModel();

            try
            {

                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = 1
                };

                var key = new PartitionKey(_partitionId);
                UserPrivateDataModel model = await RepositoryContainer.ReadItemAsync<UserPrivateDataModel>(id, key,
                    new ItemRequestOptions { });
                if(model != null)
                {
                    result.Scopes = model.Scopes;
                    result.Tenants = model.Tenants;
                    result.ValidEmail = model.ValidEmail;
                    result.Block = model.Block;
                }
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

            return _result.Create(ok, message, result);
        }

        public async Task<Result<string>> BlockUser(UserPrivateDataModel model)
        {
            bool ok = false;
            string message = "";

            try
            {
                model.ExpirationBlock = DateTime.UtcNow.AddYears(100);
                model.TimeModified = DateTime.Now;
                var key = new PartitionKey(_partitionId);
                await RepositoryContainer.ReplaceItemAsync<UserPrivateDataModel>(model, model.id, key,
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

            return _result.Create(ok, message, "");
        }

        public async Task<Result<List<AdditionalModel>>> GetBatchAdditional(List<string> ids)
        {
            bool ok = false;
            string message = "";
            List<AdditionalModel> result = new List<AdditionalModel>();

            try
            {

                var queryOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionId),
                    MaxItemCount = ids.Count
                };
                var change = "";
                foreach(var item in ids)
                {
                    change += "'" + item + "',";
                }
                change = change.Remove(change.Length - 1);
                var query = $"select * from d where ARRAY_CONTAINS([" + change +  "], d.id)";

                using (FeedIterator<UserPrivateDataModel> feedIterator = RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                   query,
                   null,
                   queryOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var item in await feedIterator.ReadNextAsync())
                        {
                            AdditionalModel modelAdd = new AdditionalModel();
                            modelAdd.Id = item.id;
                            modelAdd.Scopes = item.Scopes;
                            modelAdd.Tenants = item.Tenants;
                            modelAdd.Block = item.Block;
                            modelAdd.ValidEmail = item.ValidEmail;
                            result.Add(modelAdd);
                        }
                    }
                }

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

            return _result.Create(ok, message, result);
        }

       
    }
}
