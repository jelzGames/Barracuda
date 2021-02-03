using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Models
{
    public class RefreshTokensModel
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
