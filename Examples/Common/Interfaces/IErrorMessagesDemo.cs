using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IErrorMessagesDemo
    {
        public string Found { get; }
        public string NotFound { get; }
        public string NotAuthorized { get; }
        public string ValuesNotValid { get; }
        public string SecurityTokenExpired { get; }
    }
}
