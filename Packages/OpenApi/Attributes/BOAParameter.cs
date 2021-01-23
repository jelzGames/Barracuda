using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class BOAParameter : System.Attribute
    {
        public string Name { get; set; }
        public string In { get; set; }
        public string Description { get; set; }
        public Type DataType { get; set; }
        public bool Required { get; set; }

        public BOAParameter(string Name, string In, string Description, Type DataType = null, bool Required = false)
        {
            this.Name = Name;
            this.In = In;
            this.Description = Description;
            this.DataType = DataType;
            this.Required = Required;
        }
    }
}
