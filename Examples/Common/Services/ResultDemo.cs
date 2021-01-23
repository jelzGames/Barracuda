using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Services
{
    public class ResultDemo : IResultDemo
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResultDemo<TValue> Create<TValue>(bool success, string message, TValue value)
        {
            return new ResultDemo<TValue>
            {
                Success = success,
                Message = message,
                Value = value
            };
        }
    }

    public class ResultDemo<TValue> : ResultDemo
    {
        public TValue Value { get; set; }
    }
}
