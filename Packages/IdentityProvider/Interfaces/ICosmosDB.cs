using Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ICosmosDB
    {
        string DatabaseId { get; }
        string CollectionId { get; }
        string PartitionKey { get; }
        CosmosClient Client { get; }
        void CreateDatabase();
    }
}
