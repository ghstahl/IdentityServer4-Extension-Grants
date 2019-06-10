using InternalizeIdentityServerApp;
using Microsoft.Extensions.Configuration;
using Xunit;
using XUnitHelpers;

namespace XUnitTestProject_ExtensionGrantsApp
{

    public class MyTestServerFixture : TestServerFixture<Startup>
    {
        protected override string RelativePathToHostProject => @"../../../../InternalizeIdentityServerApp";

        protected override void LoadConfigurations(IConfigurationBuilder config, string environmentName)
        {
            Program.LoadConfigurations(config, environmentName);
        }
    }
    public abstract class TestServerBaseTests : IClassFixture<MyTestServerFixture>
    { }
}