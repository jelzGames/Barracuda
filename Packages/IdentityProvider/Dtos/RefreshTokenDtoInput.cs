using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Dtos
{
    public class RefreshTokenDtoInput
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
