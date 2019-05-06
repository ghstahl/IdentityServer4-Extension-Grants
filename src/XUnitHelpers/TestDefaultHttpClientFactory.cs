
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using IdentityModelExtras;

namespace XUnitHelpers
{
    public class TestDefaultHttpClientFactory : IDefaultHttpClientFactory
    {
        public TestServer TestServer { get; set; }
        public HttpMessageHandler HttpMessageHandler => TestServer.CreateHandler();
        public HttpClient HttpClient => TestServer.CreateClient();
    }
}
