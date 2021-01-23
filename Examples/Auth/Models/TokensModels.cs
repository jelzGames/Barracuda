using System;
using System.Collections.Generic;
using System.Text;

namespace UsersSecrets.Functions.Models
{
    public class TokensModels
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Valid { get; set; }
    }
}
