using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface IRedisCache
    {
        public Task<string> GetSringValue(string id);
        public Task SetStringValue(string id, string value);
    }
}
