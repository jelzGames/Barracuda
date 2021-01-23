using Barracuda.OpenApi.Interfaces;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Barracuda.OpenApi.Services
{
    public class OpenApiBuilder : IOpenApiBuilder
    {
        public readonly IOpenApiReader _reader;
        public readonly ISettingsOpenApi _settings;

        public OpenApiBuilder(
            IOpenApiReader reader,
            ISettingsOpenApi settings
        )
        {
            _reader = reader;
            _settings = settings;
        }

        public string Build(Assembly assembly, string typeName, HttpRequestMessage request)
        {
            var serverPath = request.RequestUri.Scheme + "://" + request.RequestUri.Authority;

            var json = _reader.Read(assembly, typeName, serverPath, _settings.OAuthUrl, _settings.OAuthTokenUrl);

            return json;
        }

        public string OpenAPIAuth()
        {
            var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string path = fileInfo.Directory.Parent.FullName;

            var content = ReadResource("Auth.html");

            return content;
        }
        public string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = "Barracuda.OpenApi.Templates." + name;
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public string OpenAPIUI()
        {
            var content = ReadResource("OpenApi.html");

            var i = content.IndexOf("url: ''");
            if (i >= 0)
            {
                content = content.Remove(i, "url: ''".Length);
                content = content.Insert(i, "url: '" + _settings.BOAUrlJson + "'");
            }

            i = content.IndexOf("oauth2RedirectUrl: ''");
            if (i >= 0)
            {
                content = content.Remove(i, "oauth2RedirectUrl: ''".Length);
                content = content.Insert(i, "oauth2RedirectUrl: '" + _settings.BOARedirectAuthUrl + "'");
            }

            i = content.IndexOf("clientId: ''");
            if (i >= 0)
            {
                content = content.Remove(i, "clientId: ''".Length);
                content = content.Insert(i, "clientId: '" + _settings.OAuthClientId + "'");
            }

            i = content.IndexOf("clientSecret: ''");
            if (i >= 0)
            {
                content = content.Remove(i, "clientSecret: ''".Length);
                content = content.Insert(i, "clientSecret: '" + _settings.OAuthClientSecret + "'");
            }
            /*
            i = content.IndexOf("BarracudaAuthUrl");
            if (i >= 0)
            {
                content = content.Remove(i, "BarracudaAuthUrl".Length);
                content = content.Insert(i, _settings.BarracudaAuthUrl);
            }

            i = content.IndexOf("BarracudaRefreshTokenUrl");
            if (i >= 0)
            {
                content = content.Remove(i, "BarracudaRefreshTokenUrl".Length);
                content = content.Insert(i, _settings.BarracudaRefreshTokenUrl);
            }

            i = content.IndexOf("BarracudaRefreshUrl");
            if (i >= 0)
            {
                content = content.Remove(i, "BarracudaRefreshUrl".Length);
                content = content.Insert(i, _settings.BarracudaRefreshUrl);
            }*/

            return content;
        }

        public string Build(Assembly assembly, string typeName, string serverPath)
        {
           
            var json = _reader.Read(assembly, typeName, serverPath, _settings.OAuthUrl, _settings.OAuthTokenUrl);

            return json;
        }
    }
}
