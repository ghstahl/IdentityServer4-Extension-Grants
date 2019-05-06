using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.DataProtection;
using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace XUnitTestProject_P7CorpP7Core
{

    public class UnitTest_Utils
    {

        void ArgumentFunc(object someObject)
        {
            P7Core.Utils.Guard.ArgumentNotNull(nameof(someObject), someObject);

        }
        void ArgumentNotNullOrEmptyFunc(string value)
        {
            P7Core.Utils.Guard.ArgumentNotNullOrEmpty(nameof(value), value);

        }
        void ArgumentNotNullOrEmptyFunc(IReadOnlyCollection<string> value)
        {
            P7Core.Utils.Guard.ArgumentNotNullOrEmpty(nameof(value), value);

        }
        [Fact]
        public async Task Test_Utils_ArgumentNotNull()
        {
            Should.Throw<Exception>(() => { ArgumentFunc(null); });
            Should.NotThrow(() => { ArgumentFunc(new object() { }); });
        }
        [Fact]
        public async Task Test_Utils_ArgumentNotNullOrEmpty()
        {
            Should.Throw<Exception>(() => { ArgumentNotNullOrEmptyFunc((string)null); });
            Should.Throw<Exception>(() => { ArgumentNotNullOrEmptyFunc(""); });
            Should.NotThrow(() => { ArgumentNotNullOrEmptyFunc("hello"); });
        }
        [Fact]
        public async Task Test_Utils_ArgumentNotNullOrEmpty_IReadOnlyCollection()
        {
            IReadOnlyCollection<string> readOnlyCollection = null;
            Should.Throw<Exception>(() => { ArgumentNotNullOrEmptyFunc(readOnlyCollection); });

            readOnlyCollection = A.Fake<IReadOnlyCollection<string>>();
            A.CallTo(() => readOnlyCollection.Count).Returns(0);
            Should.Throw<Exception>(() => { ArgumentNotNullOrEmptyFunc(readOnlyCollection); });

            readOnlyCollection = A.Fake<IReadOnlyCollection<string>>();
            A.CallTo(() => readOnlyCollection.Count).Returns(1);

            Should.NotThrow(() => { ArgumentNotNullOrEmptyFunc(readOnlyCollection); });

        }
        [Fact]
        public async Task Test_Utils_ArgumentValid()
        {
            Should.Throw<Exception>(() => { P7Core.Utils.Guard.ArgumentValid(false, "value", "test"); });
            Should.NotThrow(() => { P7Core.Utils.Guard.ArgumentValid(true, "value", "test"); });

        }
        [Fact]
        public async Task Test_Utils_OperationValid()
        {
            Should.Throw<Exception>(() => { P7Core.Utils.Guard.OperationValid(false, "test"); });
            Should.NotThrow(() => { P7Core.Utils.Guard.OperationValid(true, "test"); });

        }
    }
}