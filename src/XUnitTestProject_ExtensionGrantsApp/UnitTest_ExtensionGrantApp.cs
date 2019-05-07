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
        public async Task Test_arbitrary_resource_owner_success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/auth/arbitrary_resource_owner")
            {
                // Content = new FormUrlEncodedContent(dict)
            };
            var response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            var values = JsonConvert.DeserializeObject<SafeTokenRawResult>(jsonString);
            values.ShouldNotBeNull();


        }
        public class TokenErrorRawResult
        {
            public TokenErrorResult TokenErrorResult { get; set; }

        }

        [Fact]
        public async Task Test_arbitrary_resource_owner_bad_client_success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/auth/arbitrary_resource_owner_bad_client")
            {
                // Content = new FormUrlEncodedContent(dict)
            };
            var response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var value = JsonConvert.DeserializeObject<SafeTokenRawResult>(jsonString, settings);
            value.ShouldNotBeNull();
            value.TokenErrorResult.ShouldNotBeNull();


        }

    }
}
