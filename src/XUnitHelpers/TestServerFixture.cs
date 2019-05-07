using System;
using System.IO;
using System.Net.Http;
using IdentityModelExtras;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;

namespace XUnitHelpers
{
    public abstract class TestServerFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly TestServer TestServer;
        public HttpClient Client { get; }
        public HttpMessageHandler MessageHandler { get; }

        // RelativePathToHostProject = @"..\..\..\..\TheWebApp";
        protected abstract string RelativePathToHostProject { get; }

        public TestServerFixture()
        {
            var contentRootPath = GetContentRootPath();
            var builder = new WebHostBuilder()
                .UseContentRoot(contentRootPath)
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.TryAddTransient<IDefaultHttpClientFactory>(serviceProvider =>
                    {
                        return new TestDefaultHttpClientFactory()
                        {
                            TestServer = TestServer
                        };
                    });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    LoadConfigurations(config, environmentName);

                })
                .UseStartup<TStartup>(); // Uses Start up class from your API Host project to configure the test server

            TestServer = new TestServer(builder);
            Client = TestServer.CreateClient();
            MessageHandler = TestServer.CreateHandler();

        }

        protected abstract void LoadConfigurations(IConfigurationBuilder config, string environmentName);

        private string GetContentRootPath()
        {
            var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            return Path.Combine(testProjectPath, RelativePathToHostProject);
        }

        public void Dispose()
        {
            Client.Dispose();
            TestServer.Dispose();
        }
    }
}
