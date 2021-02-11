using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Models
{
    public class AdditionalModel
    {
       public List<string> Scopes { get; set; }
       public List<string> Tenants { get; set; }
       public bool Block { get; set; }

        public AdditionalModel()
        {
            Scopes = Scopes != null ? Scopes : null;
            Tenants = Tenants != null ? Tenants : null;
        }
    }
}
