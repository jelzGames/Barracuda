using Barracuda.Indentity.Provider.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Services
{
    public class SettingsCosmos : ISettingsCosmos
    {
        public SettingsCosmos()
        {
            DatabaseEndpoint = System.Environment.GetEnvironmentVariable("DatabaseEndpoint", EnvironmentVariableTarget.Process);
            DatabaseSecretKey = System.Environment.GetEnvironmentVariable("DatabaseSecretKey", EnvironmentVariableTarget.Process);
            DatabaseId = System.Environment.GetEnvironmentVariable("DatabaseId", EnvironmentVariableTarget.Process);
            CollectionId = System.Environment.GetEnvironmentVariable("CollectionId", EnvironmentVariableTarget.Process);
            PartitionKeyPath = System.Environment.GetEnvironmentVariable("PartitionKeyPath", EnvironmentVariableTarget.Process);
        }

        public string DatabaseEndpoint { get; private set; }

        public string DatabaseSecretKey { get; private set; }

        public string DatabaseId { get; private set; }

        public string CollectionId { get; private set; }

        public string PartitionKeyPath { get; private set; }
    }
}
