using Barracuda.Indentity.Provider.Interfaces;

namespace Barracuda.Indentity.Provider.Services
{
    public class ErrorMessages : IErrorMessages
    {
        public string Found { get => "Found"; }

        public string NotFound { get => "NotFound"; }

        public string NotAuthorized { get => "NotAuthorized"; }

        public string ValuesNotValid { get => "ValuesNotValid"; }

        public string SecurityTokenExpired { get => "SecurityTokenExpired"; }

        public string ValidateTokenConfirmEmailExpired { get => "ValidateTokenConfirmEmailExpired"; }

        public string Block { get => "Block"; }
    }
}
