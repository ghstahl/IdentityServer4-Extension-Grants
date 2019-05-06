using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace XUnitTest_P7Corp.P7CoreWebApp
{
    public class UnitTestAppConfiguration : IClassFixture<MyTestServerFixture>
    {
        private readonly MyTestServerFixture _fixture;

        public UnitTestAppConfiguration(MyTestServerFixture fixture)
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
        public async Task Test_Random_upper_lower_case_GET_success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/tEsThArNeSs")
            {
                // Content = new FormUrlEncodedContent(dict)
            };
            var response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            var values = JsonConvert.DeserializeObject<List<string>>(jsonString);
            values.ShouldNotBeNull();
            values.Count.ShouldBeGreaterThan(0);

        }

    }
}
