using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Models
{
    public class AdditionalModel
    {
        public string Id { get; set; }
        public List<string> Scopes { get; set; }
       public List<string> Tenants { get; set; }
       public bool Block { get; set; }
       
        public bool ValidEmail { get; set; }

        public AdditionalModel()
        {
            Scopes = Scopes != null ? Scopes : null;
            Tenants = Tenants != null ? Tenants : null;
        }
    }
}
