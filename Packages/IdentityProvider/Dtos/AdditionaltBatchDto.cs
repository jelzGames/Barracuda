using System;
using System.Collections.Generic;
using System.Text;

namespace Barracuda.Indentity.Provider.Dtos
{
    public class AdditionaltBatchDto
    {
        public List<string> Batch { get; set; }

        public AdditionaltBatchDto()
        {
            Batch = Batch != null ? Batch : new List<string>();
        }

    }
}
