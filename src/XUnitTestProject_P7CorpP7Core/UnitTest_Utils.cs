using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.DataProtection;
using P7Core.Extensions;
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
        public void Test_StringExtensions_To_From_IEnumberable()
        {
            List<string> ll = new List<string>()
            {
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString()
            };
            var a = ll.ToSpaceSeparatedString();
            var aa = a.FromSpaceSeparatedString();
            var bb = aa.ToSpaceSeparatedString();
            (bb == a).ShouldBeTrue();

            IEnumerable<string> ee = null;
            var c = ee.ToSpaceSeparatedString();
            (c == "").ShouldBeTrue();
        }

        [Fact]
        public void Test_StringExtensions_AsGuid()
        {
            var a = Guid.NewGuid().ToString();
            var g = a.AsGuid();
            var aa = g.ToString();
            (aa ==a).ShouldBeTrue();
           
        }

        [Fact]
        public void Test_StringExtensions()
        {
            "dog".IsPresent().ShouldBeTrue();

            string goodUrl = "https://a.b.com/";
            var fixedUrl = goodUrl.EnsureTrailingSlash();
            fixedUrl.ShouldBeSameAs(goodUrl);
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