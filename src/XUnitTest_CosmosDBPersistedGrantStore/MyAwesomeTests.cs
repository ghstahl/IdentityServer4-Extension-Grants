using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Shouldly;
using Xunit;
using XUnitTest_CosmosDBPersistedGrantStore.TestCaseOrdering;

namespace XUnitTest_CosmosDBPersistedGrantStore
{
    [TestCaseOrderer("XUnitTest_CosmosDBPersistedGrantStore.TestCaseOrdering.PriorityOrderer", "XUnitTest_CosmosDBPersistedGrantStore")]
    public class MyAwesomeTests
    {
        private readonly IDependency _d;
        private IPersistedGrantStore _persistedGrantStore;
        private string _key { get; set; }
        private string _clientId { get; set; }
        public MyAwesomeTests(IPersistedGrantStore persistedGrantStore,
            IDependency d)
        {
            _d = d;
            _persistedGrantStore = persistedGrantStore;
            _key = "__the_key";
            _clientId = "__the_client";

        }
        string NewGuidS => Guid.NewGuid().ToString();
        [Fact]
        public void AssertThatWeDoStuff()
        {
            Assert.Equal(1, _d.Value);
        }
        async Task StoreAsync(PersistedGrant persistedGrant)
        {
            await _persistedGrantStore.StoreAsync(persistedGrant);
        }
        async Task<PersistedGrant> GetAsync(string key)
        {
            var actual = await _persistedGrantStore.GetAsync(key);
            return actual;
        }
        [Fact, TestPriority(0)]
        public async Task success_persist()
        {
            var persistedGrant = new PersistedGrant()
            {
                ClientId = _clientId,
                Data = NewGuidS,
                Key = _key,
                SubjectId = NewGuidS,
                Type = NewGuidS,
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddMinutes(10)
            };
            Should.NotThrow(() =>
            {
                StoreAsync(persistedGrant).GetAwaiter();
            });
        }
        [Fact, TestPriority(1)]
        public async Task success_get()
        {
            PersistedGrant persistedGrant = null;
            Should.NotThrow(() =>
            {
                persistedGrant = GetAsync(_key).GetAwaiter().GetResult();
            });
            persistedGrant.ShouldNotBeNull();
            (persistedGrant.Key == _key).ShouldBeTrue();
            persistedGrant.Key.ShouldNotBeNullOrWhiteSpace();
            persistedGrant.Key.ShouldBe(_key);

            persistedGrant.ClientId.ShouldBe(_clientId);
        }
        [Fact, TestPriority(2)]
        public async Task success_remove()
        {
            Should.NotThrow(() =>
            {
                _persistedGrantStore.RemoveAsync(_key).GetAwaiter();
            });
        }
        [Fact, TestPriority(3)]
        public async Task success_not_found()
        {
            PersistedGrant persistedGrant = null;
            Should.NotThrow(() =>
            {
                persistedGrant = GetAsync(_key).GetAwaiter().GetResult();
            });
            persistedGrant.ShouldBeNull();
        }
    }

}
