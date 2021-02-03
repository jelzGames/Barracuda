using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Users.Domain.Models
{
    public class UserModel : BaseModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string Licence { get; set; }
        public string insurance { get; set; }
        public bool Photo { get; set; }
        public string identification { get; set; }
    }
}
