﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOAHttpPost : System.Attribute
    {
        public BOAHttpPost()
        {
        }
    }
}
