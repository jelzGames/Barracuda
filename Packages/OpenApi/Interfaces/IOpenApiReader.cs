using System.Reflection;

namespace Barracuda.OpenApi.Interfaces
{
    public interface IOpenApiReader
    {
        public string Read(Assembly assembly, string controller, string server, string authUrl, string tokenUrl);
    }
}
