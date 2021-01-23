using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class QueryAllModel<TValue>
    {
        public IEnumerable<TValue> models { get; set; }
        public string ContinuationToken { get; set; }
    }
}
