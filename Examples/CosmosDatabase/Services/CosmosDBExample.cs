using Azure.Cosmos;
using CosmosDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDatabase.Services
{
    public class CosmosDBExample : ICosmosDBExample
    {
        string _partitionKeyPath;
        public string PartitionKey => _partitionKeyPath;
        string _collectionId;
        public string CollectionId => _collectionId;
        string _databaseId;
        public string DatabaseId => _databaseId;
        CosmosClient _client;
        public CosmosClient Client => _client;
        int _maxItemCount;
        public int MaxItemCount => _maxItemCount;

        public CosmosDBExample(
            string endpoint = "",
            string secretKey = "",
            string databaseId = "",
            int maxItemCount = 0
        )
        {
            _partitionKeyPath = "/PartitionId";
            _collectionId = "Delivers";
            _databaseId = databaseId;
            _maxItemCount = maxItemCount;
            _client = new CosmosClient(endpoint, secretKey);
        }

        public async void CreateDatabase()
        {

            Database db = await _client.CreateDatabaseIfNotExistsAsync(_databaseId, 400);

            var containerResponse = await db.CreateContainerIfNotExistsAsync(_collectionId, _partitionKeyPath);
        }
    }
}
