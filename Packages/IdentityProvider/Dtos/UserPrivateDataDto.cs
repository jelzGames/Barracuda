﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Dtos
{
    public class UserPrivateDataDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool ValidEmail { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Scopes { get; set; }
        public List<string> Tenants { get; set; }
        public bool Block { get; set; }
        public DateTime ExpirationLock { get; set; }

        public UserPrivateDataDto()
        {
            Scopes = Scopes != null ? Scopes : new List<string>();
            Tenants = Tenants != null ? Tenants : new List<string>();
        }
    }
}
