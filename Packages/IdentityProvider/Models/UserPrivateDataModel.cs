using Barracuda.Indentity.Provider.Models;
using System.Collections.Generic;

namespace Barracuda.Indentity.Provide.Models
{
    public class UserPrivateDataModel : BaseModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public bool ValidEmail { get; set; }
        public dynamic Scopes { get; set; }
    }
}
