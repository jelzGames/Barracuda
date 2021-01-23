using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class QueryInputModel
    {
        public string Query { get; set; }

        public string ContinuationToken { get; set; }
    }
}
