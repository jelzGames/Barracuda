using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOAInfo : System.Attribute
    {
        public string Title { get; set; }
        public string Version { get; set; }

        public BOAInfo(string Title, string Version)
        {
            this.Title = Title;
            this.Version = Version;
        }
       
    }
}
