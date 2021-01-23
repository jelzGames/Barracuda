using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ISettingsCosmos
    {
        string DatabaseEndpoint { get; }
        string DatabaseSecretKey { get; }
        string DatabaseId { get; }
        string CollectionId { get; }
        string PartitionKeyPath { get; }
    }
}
