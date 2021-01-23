using Azure.Cosmos;
using Barracuda.Indentity.Provide.Models;
using Barracuda.Indentity.Provider.Interfaces;
using System;
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
                
                var query = $"select * from Delivers d where d.Email = '{email}'";
                await foreach (var page in RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query, null, queryOptions, new CancellationToken()).AsPages())
                {
                    if (page.Values.Count > 0)
                    {
                        model = page.Values[0];
                    }
                   
                    break;
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

        public async Task<Result<string>> Register(string email, string password)
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
                var query = $"select d.id, d.Email from Delivers d where d.Email = '{email}'";
                await foreach (var page in RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query, null, queryOptions, new CancellationToken()).AsPages())
                {
                    isFound = page.Values.Count > 0 ? true : false;
                    if (isFound)
                    {
                        id = page.Values[0].id;
                    }
                    break;
                }

                if (isFound)
                {
                    message = _errors.Found;
                }
                else
                {
                    var item = new UserPrivateDataModel();
                    UpdatePrivateMetadata(item, email, password, false);
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
                var query = $"select * from Delivers d where d.Email = '{email}'";
                await foreach (var page in RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query, null, queryOptions, new CancellationToken()).AsPages())
                {
                    isFound = page.Values.Count > 0 ? true : false;
                    if (isFound)
                    {
                        model = page.Values[0];
                    }
                    break;
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

        private void UpdatePrivateMetadata(UserPrivateDataModel item, string email, string password, bool isUpdate)
        {
            if (!isUpdate)
            {
                item.id = Guid.NewGuid().ToString();
                item.Email = email;
                item.TimeCreated = DateTime.Now;
                item.PartitionId = _partitionId;
            }
            else
            {
                item.TimeModified = DateTime.Now;
            }

            item.Password = _crypto.GetStringSha256Hash(email + password + _settings.SecretKey);
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
                var query = $"select * from Delivers d where d.id = '{id}'";
                await foreach (var page in RepositoryContainer.GetItemQueryIterator<UserPrivateDataModel>(
                    query, null, queryOptions, new CancellationToken()).AsPages())
                {
                    if (page.Values.Count > 0)
                    {
                        model = page.Values[0];
                        isFound = true;
                    }
                    break;
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
    }
}
