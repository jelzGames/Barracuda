﻿using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDatabase.Interfaces
{
    public interface ICosmosDB
    {
        string DatabaseId { get; }
        string CollectionId { get; }
        string PartitionKey { get; }
        CosmosClient Client { get; }
        int MaxItemCount { get; }
        void CreateDatabase();
    }
}
