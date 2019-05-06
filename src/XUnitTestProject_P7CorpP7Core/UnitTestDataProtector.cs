using System;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using P7Core.DataProtector;
using Shouldly;
using Xunit;

namespace XUnitTestProject_P7CorpP7Core
{
    public class TestDataProtector : DataProtectorBase
    {
        public TestDataProtector(IDataProtectionProvider provider, string purpose, ILogger logger) : base(provider, purpose, logger)
        {
        }
    }

    public class UnitTestDataProtector
    {
        [Fact]
        public async Task Test_DataProtectorAsync()
        {
            string purpose = "test";

            var actual = "hello";
            byte[] bytesActual = Encoding.ASCII.GetBytes(actual);
            var fakeProtected = new byte[] { };
            var dataProtectionProvider = A.Fake<IDataProtectionProvider>();
            var dataProtector = A.Fake<IDataProtector>();
            A.CallTo(() => dataProtectionProvider.CreateProtector(purpose)).Returns(dataProtector);

            A.CallTo(() => dataProtector.Protect(A<byte[]>.Ignored)).Returns(bytesActual);

            A.CallTo(() => dataProtector.Unprotect(A<byte[]>.Ignored)).Returns(bytesActual);
            var logger = A.Fake<ILogger>();
            var testDataProtector = new TestDataProtector(dataProtectionProvider, purpose, logger);


            var protectedData = await testDataProtector.ProtectAsync(actual);
            protectedData.ShouldNotBeNullOrEmpty();
            var expected = await testDataProtector.UnprotectAsync(protectedData);
            actual.ShouldBe(expected);
        }
        [Fact]
        public async Task Test_DataProtector_Throw_Protect()
        {
            string purpose = "test";

            var actual = "hello";
            byte[] bytesActual = Encoding.ASCII.GetBytes(actual);
            var fakeProtected = new byte[] { };
            var dataProtectionProvider = A.Fake<IDataProtectionProvider>();
            var dataProtector = A.Fake<IDataProtector>();
            A.CallTo(() => dataProtectionProvider.CreateProtector(purpose)).Returns(dataProtector);

            A.CallTo(() => dataProtector.Protect(A<byte[]>.Ignored)).Throws<Exception>();

            A.CallTo(() => dataProtector.Unprotect(A<byte[]>.Ignored)).Throws<Exception>();
            var logger = A.Fake<ILogger>();
            var testDataProtector = new TestDataProtector(dataProtectionProvider, purpose, logger);

            Should.Throw<Exception>(() =>
            {
                testDataProtector.ProtectAsync(actual);
            });

        }
        [Fact]
        public async Task Test_DataProtector_Throw_UnProtect()
        {
            string purpose = "test";

            var actual = "hello";
            byte[] bytesActual = Encoding.ASCII.GetBytes(actual);
            var fakeProtected = new byte[] { };
            var dataProtectionProvider = A.Fake<IDataProtectionProvider>();
            var dataProtector = A.Fake<IDataProtector>();
            A.CallTo(() => dataProtectionProvider.CreateProtector(purpose)).Returns(dataProtector);

            A.CallTo(() => dataProtector.Protect(A<byte[]>.Ignored)).Returns(bytesActual);

            A.CallTo(() => dataProtector.Unprotect(A<byte[]>.Ignored)).Throws<Exception>();
            var logger = A.Fake<ILogger>();
            var testDataProtector = new TestDataProtector(dataProtectionProvider, purpose, logger);

            var protectedData = await testDataProtector.ProtectAsync(actual);
            protectedData.ShouldNotBeNullOrEmpty();

            Should.Throw<Exception>(() =>
            {
                testDataProtector.UnprotectAsync(protectedData);
            });

        }
    }
}
