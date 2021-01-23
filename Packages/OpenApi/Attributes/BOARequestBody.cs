using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.OpenApi.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class BOARequestBody : System.Attribute
    {
        public Type DataType;
        public string Description;
        public string DictionaryName;
        public string DictionaryArg0;
        public string DictionaryArg1;


        public BOARequestBody(Type DataType, string Description, string DictionaryName = "", string DictionaryArg0 = "", string DictionaryArg1 = "")
        {
            this.DataType = DataType;
            this.Description = Description;
            this.DictionaryName = DictionaryName;
            this.DictionaryArg0 = DictionaryArg0;
            this.DictionaryArg1 = DictionaryArg1;
        }
    }
}
