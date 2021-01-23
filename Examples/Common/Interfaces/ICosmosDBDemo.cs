using Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface ICosmosDBDemo
    {
        string DatabaseId { get; }
        string CollectionId { get; }
        string PartitionKey { get; }
        CosmosClient Client { get; }
        int MaxItemCount { get; }
        void CreateDatabase();
    }
}
