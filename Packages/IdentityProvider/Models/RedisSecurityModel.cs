using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Models
{
    public class RedisSecurityModel
    {
        public bool Block { get; set; }
        public List<RefreshTokensModel> Tokens { get; set; }
    }
}
