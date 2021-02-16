using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Models
{
    public class RedisSecurityModel
    {
        public bool Block { get; set; }
        public List<RefreshTokensModel> Tokens { get; set; }
        public List<string> Scopes { get; set; }
        public List<string> Tenants { get; set; }
        public bool ValidEmail { get; set; }

        public RedisSecurityModel()
        {
            Tokens = Tokens != null ? Tokens : new List<RefreshTokensModel>();
            Scopes = Scopes != null ? Scopes : new List<string>();
            Tenants = Tenants != null ? Tenants : new List<string>();

        }

    }
}
