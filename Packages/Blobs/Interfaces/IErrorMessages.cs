using System;
using System.Collections.Generic;
using System.Text;

namespace Bases.Interfaces
{
    public interface IErrorMessages
    {
        public string Found { get; }
        public string NotFound { get; }
        public string NotAuthorized { get; }
        public string ValuesNotValid { get; }
        public string SecurityTokenExpired { get; }
    }
}
