using Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IResultDemo
    {
        public ResultDemo<TValue> Create<TValue>(bool success, string message, TValue value);
    }
}
