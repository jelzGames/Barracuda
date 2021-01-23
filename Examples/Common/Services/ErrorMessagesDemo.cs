using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Services
{
    public class ErrorMessagesDemo : IErrorMessagesDemo
    {
        public string Found { get => "Found"; }

        public string NotFound { get => "NotFound"; }

        public string NotAuthorized { get => "NotAuthorized"; }

        public string ValuesNotValid { get => "ValuesNotValid"; }

        public string SecurityTokenExpired { get => "SecurityTokenExpired"; }
    }
}
