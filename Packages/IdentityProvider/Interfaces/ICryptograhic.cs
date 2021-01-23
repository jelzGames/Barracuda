using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface ICryptograhic
    {
        public string GetStringSha256Hash(string text);
        public string GetRandomNumber();
    }
}
