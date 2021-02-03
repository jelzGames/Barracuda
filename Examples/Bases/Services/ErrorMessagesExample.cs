using Bases.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bases.Services
{
    public class ErrorMessagesExample : IErrorMessagesExample
    {
        public string Found { get => "Found"; }

        public string NotFound { get => "NotFound"; }

        public string NotAuthorized { get => "NotAuthorized"; }

        public string ValuesNotValid { get => "ValuesNotValid"; }

        public string SecurityTokenExpired { get => "SecurityTokenExpired"; }
    }
}
