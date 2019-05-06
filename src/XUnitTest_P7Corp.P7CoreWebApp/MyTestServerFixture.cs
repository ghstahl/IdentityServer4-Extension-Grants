using Microsoft.Extensions.Configuration;
using P7Corp.P7CoreWebApp;
using Xunit;
using XUnitHelpers;

namespace XUnitTest_P7Corp.P7CoreWebApp
{

    public class MyTestServerFixture : TestServerFixture<Startup>
    {
        protected override string RelativePathToHostProject => @"..\..\..\..\P7Corp.P7CoreWebApp";

        protected override void LoadConfigurations(IConfigurationBuilder config, string environmentName)
        {
            Program.LoadConfigurations(config, environmentName);
        }
    }
    public abstract class TestServerBaseTests : IClassFixture<MyTestServerFixture>
    { }
}