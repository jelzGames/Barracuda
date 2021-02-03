using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class BaseModel
    {
        public string id { get; set; }
        public string PartitionId { get; set; }
        public string IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? TimeCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? TimeModified { get; set; }
        public string _etag { get; set; }
        public string _self { get; set; }
        public IQueryable<string> Tags { get; set; }

        public BaseModel()
        {
            Tags = Tags ?? new List<string>().AsQueryable();
        }

    }
}
