using System;
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

    }
}
