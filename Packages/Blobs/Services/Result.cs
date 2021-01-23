using Bases.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bases.Services
{
    public class Result : IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public Result<TValue> Create<TValue>(bool success, string message, TValue value)
        {
            return new Result<TValue>
            {
                Success = success,
                Message = message,
                Value = value
            };
        }
    }

    public class Result<TValue> : Result
    {
        public TValue Value { get; set; }
    }
}
