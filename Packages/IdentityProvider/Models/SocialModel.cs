using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Models
{
    public class SocialModel
    {   
        public string id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public List<string> Scopes { get; set; }
        public List<string> Tenants { get; set; }

        public SocialModel()
        {
            Scopes = Scopes != null ? Scopes : new List<string>();
            Tenants = Tenants != null ? Tenants : new List<string>();
        }
    }
}
