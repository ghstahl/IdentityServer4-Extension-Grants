using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4Extras.Endpoints;
using InternalizeIdentityServerApp.Controllers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace XUnitTestProject_ExtensionGrantsApp
{

    public partial class UnitTest_ExtensionGrantApp : IClassFixture<MyTestServerFixture>
    {
        private readonly MyTestServerFixture _fixture;

        public UnitTest_ExtensionGrantApp(MyTestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void AssureFixture()
        {
            _fixture.ShouldNotBeNull();
            var client = _fixture.Client;
            client.ShouldNotBeNull();

            var messageHandler = _fixture.MessageHandler;
            messageHandler.ShouldNotBeNull();

        }
        [Fact]
        public async Task Test_index_success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "")
            {
                // Content = new FormUrlEncodedContent(dict)
            };
            var response = await client.SendAsync(req);

        }
    }
}
