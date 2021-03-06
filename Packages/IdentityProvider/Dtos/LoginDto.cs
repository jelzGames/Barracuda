﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Dtos
{
    public class LoginDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool ValidEmail { get; set; }
        public List<string> Scopes { get; set; }
        public List<string> Tenants { get; set; }
        public bool Block { get; set; }
    }
}
