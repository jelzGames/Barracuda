﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Dtos
{
    public class LoginDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public dynamic Scopes { get; set; }
    }
}
