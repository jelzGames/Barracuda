using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOAFunctionName : System.Attribute
    {
        public string Name { get; set; }

        public BOAFunctionName(string Name)
        {
            this.Name = Name;
        }
    }
}