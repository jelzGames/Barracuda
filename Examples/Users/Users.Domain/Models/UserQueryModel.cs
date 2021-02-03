using System;
using System.Collections.Generic;
using System.Text;

namespace Users.Domain.Models
{
    public class UserQueryModel
    {
        public IEnumerable<UserModel> models { get; set; }
        public string ContinuationToken { get; set; }
    }
}
