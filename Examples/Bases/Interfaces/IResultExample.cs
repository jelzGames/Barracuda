using Bases.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bases.Interfaces
{
    public interface IResultExample
    {
        public Result<TValue> Create<TValue>(bool success, string message, TValue value);
    }
}
