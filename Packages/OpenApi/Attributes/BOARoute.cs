using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
   [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOARoute : System.Attribute
    {
        public string Template { get; set; }

        public BOARoute(string Template)
        {
            this.Template = Template;
        }
    }
}
