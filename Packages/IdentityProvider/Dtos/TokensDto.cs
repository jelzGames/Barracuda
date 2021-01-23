using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Dtos
{
    public class TokensDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Valid { get; set; }
    }
}
