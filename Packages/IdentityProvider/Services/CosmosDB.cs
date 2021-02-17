
using Barracuda.Indentity.Provider.Interfaces;
using Microsoft.Azure.Cosmos;
using System;

namespace Barracuda.Indentity.Provider.Services
{
    public class CosmosDB : ICosmosDB
    {
        string _partitionKeyPath;
        public string PartitionKey => _partitionKeyPath;
        string _collectionId;
        public string CollectionId => _collectionId;
        string _databaseId;
        public string DatabaseId => _databaseId;
        CosmosClient _client;
        public CosmosClient Client => _client;
       
        public CosmosDB(
            ISettingsCosmos settings
        )
        {
            _partitionKeyPath = settings.PartitionKeyPath;
            _collectionId = settings.CollectionId;
            _databaseId = settings.DatabaseId;
            _client = new CosmosClient(settings.DatabaseEndpoint, settings.DatabaseSecretKey);
        }

        public async void CreateDatabase()
        {

            Database db = await _client.CreateDatabaseIfNotExistsAsync(_databaseId, 400);

            var containerResponse = await db.CreateContainerIfNotExistsAsync(_collectionId, _partitionKeyPath);
        }
    }
}
