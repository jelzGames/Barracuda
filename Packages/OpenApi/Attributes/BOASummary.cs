using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOASummary : System.Attribute
    {
        public string description;
     
        public BOASummary(string description)
        {
            this.description = description;
        }
    }
}
