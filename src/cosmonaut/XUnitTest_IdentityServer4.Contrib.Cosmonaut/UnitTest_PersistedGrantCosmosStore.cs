using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Response;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Stores;
using Microsoft.Azure.Documents;
using Xunit;
using XUnitHelpers.TestCaseOrdering;

namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{
    [TestCaseOrderer("XUnitHelpers.TestCaseOrdering.PriorityOrderer", "XUnitHelpers")]
    public class UnitTest_PersistedGrantCosmosStore
    {
        string NewGuidS => Guid.NewGuid().ToString();
      
        private ICosmonautClient _cosmonautClient;

        private ICosmosStore<PersistedGrantEntity> _persistedGrantCosmosStore;
        private static string _currentId;
        public static readonly string DatabaseId = $"DB{nameof(UnitTest_PersistedGrantCosmosStore)}";
        public static readonly string PersistantGrantCollectionName = $"COL{nameof(UnitTest_PersistedGrantCosmosStore)}_PersistedGrant";
        private IPersistedGrantStore _persistedGrantStore;
        public UnitTest_PersistedGrantCosmosStore(
            ICosmonautClient cosmonautClient,
            ICosmosStore<PersistedGrantEntity> persistedGrantCosmosStore,
            IPersistedGrantStore persistedGrantStore)
        {
            _cosmonautClient = cosmonautClient;
            _persistedGrantCosmosStore = persistedGrantCosmosStore;
            _persistedGrantStore = persistedGrantStore;
        }

        [Fact, TestPriority(0)]
        public async Task Persist_Grant_That_Will_Expire()
        {
            _currentId = NewGuidS;
            var ttl = 3; // 2 seconds
            var entity = new PersistedGrantEntity
            {
                Key = _currentId,
                ClientId = NewGuidS,
                CreationTime = DateTime.UtcNow,
                Data = NewGuidS,
                Expiration = DateTime.UtcNow.AddSeconds(ttl),
                Type = NewGuidS,
                TTL = ttl
            };


            var response = await _persistedGrantCosmosStore.AddAsync(entity);

            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.CosmosOperationStatus.Should().Be(CosmosOperationStatus.Success);
        }
        [Fact, TestPriority(1)]
        public async Task Persist_Grant_That_Will_Expire_Fetch_SUCCESS()
        {
            var entity = await _persistedGrantCosmosStore.FindAsync(_currentId, _currentId);

            entity.Should().NotBeNull();
            entity.Key.Should().Be(_currentId);
            Thread.Sleep(5000);

            entity = await _persistedGrantCosmosStore.FindAsync(_currentId, _currentId);
            entity.Should().BeNull();
        }

    }
}
