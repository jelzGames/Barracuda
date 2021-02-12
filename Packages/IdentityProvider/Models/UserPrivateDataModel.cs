using Barracuda.Indentity.Provider.Models;
using System;
using System.Collections.Generic;

namespace Barracuda.Indentity.Provide.Models
{
    public class UserPrivateDataModel : BaseModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<RefreshTokensModel> RefreshTokens { get; set; }
        public bool ValidEmail { get; set; }
        public List<string> Scopes { get; set; }
        public List<string> Tenants { get; set; }
        public bool Block { get; set; }
        public DateTime ExpirationBlock { get; set; }

        public UserPrivateDataModel()
        {
            RefreshTokens = RefreshTokens == null ? new List<RefreshTokensModel>() : RefreshTokens;
            Scopes = Scopes != null ? Scopes : new List<string>();
            Tenants = Tenants != null ? Tenants : new List<string>();
        }
    }
}
