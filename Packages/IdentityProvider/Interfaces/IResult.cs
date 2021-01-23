using Barracuda.Indentity.Provider.Services;

namespace Barracuda.Indentity.Provider.Interfaces
{
    public interface IResult
    {
        public Result<TValue> Create<TValue>(bool success, string message, TValue value);
    }
}
