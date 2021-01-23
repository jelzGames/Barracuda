using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOAVersion : System.Attribute
    {
        public string Version { get; set; }

        public BOAVersion(string Version)
        {
            this.Version = Version;
        }
    }
}
