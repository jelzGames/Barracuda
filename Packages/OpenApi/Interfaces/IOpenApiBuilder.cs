using System.Net.Http;
using System.Reflection;

namespace Barracuda.OpenApi.Interfaces
{
    public interface IOpenApiBuilder
    {
        public string Build(Assembly assembly, string typeName);
        public string Build(Assembly assembly, string typeName, string serverPath);
        public string OpenAPIUI();
        public string OpenAPIAuth();
    }
}
