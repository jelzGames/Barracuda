using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ISettingsRedis
    {
        string RedisCacheConnection { get; }
    }
}
