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
        string NewGuidS => Guid.NewGuid().ToString() + "/a";

        private DatabaseInitializer<UnitTest_PersistedGrantStore> _databaseInitializer;
        private ICosmonautClient _cosmonautClient;

        private ICosmosStore<PersistedGrantEntity> _persistedGrantCosmosStore;
        private static string _currentId;

        private IPersistedGrantStore _persistedGrantStore;
        private static PersistedGrantEntity _currentEntity;

        public UnitTest_PersistedGrantCosmosStore(
            DatabaseInitializer<UnitTest_PersistedGrantStore> databaseInitializer,
            ICosmonautClient cosmonautClient,
            ICosmosStore<PersistedGrantEntity> persistedGrantCosmosStore,
            IPersistedGrantStore persistedGrantStore)
        {
            _databaseInitializer = databaseInitializer;
            _cosmonautClient = cosmonautClient;
            _persistedGrantCosmosStore = persistedGrantCosmosStore;
            _persistedGrantStore = persistedGrantStore;
        }

        [Fact, TestPriority(-1000)]
        public async Task Ensure_Database_ScalingSettings()
        {


            // Act
            var result = new Action(() =>
            {
                _databaseInitializer.action();
            });

            //Assert
            result.Should().NotThrow();

        }

        [Fact, TestPriority(0)]
        public async Task Persist_Grant_That_Will_Expire()
        {

            var ttl = 5; // 2 seconds
            _currentEntity = new PersistedGrantEntity
            {
                Key = NewGuidS,
                ClientId = NewGuidS,
                CreationTime = DateTime.UtcNow,
                Data = NewGuidS,
                Expiration = DateTime.UtcNow.AddSeconds(ttl),
                Type = NewGuidS,
                TTL = ttl
            };


            var response = await _persistedGrantCosmosStore.AddAsync(_currentEntity);

            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.CosmosOperationStatus.Should().Be(CosmosOperationStatus.Success);
        }
        [Fact, TestPriority(1)]
        public async Task Persist_Grant_That_Will_Expire_Fetch_SUCCESS()
        {

            var sql = $"SELECT* FROM c where c.key = \"{_currentEntity.Key}\"";
            var entity = (await _persistedGrantCosmosStore.QuerySingleAsync(sql, feedOptions: new Microsoft.Azure.Documents.Client.FeedOptions
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(_currentEntity.Key)
            }));
            entity.Should().NotBeNull();
            _currentEntity.Key.Should().Be(entity.Key);

            _currentEntity.Id = entity.Id;

            entity = await _persistedGrantCosmosStore.FindAsync(entity.Id, _currentEntity.Key);

            entity.Should().NotBeNull();
            entity.Key.Should().Be(_currentEntity.Key);

        }
        [Fact, TestPriority(2)]
        public async Task Persist_Grant_That_Will_Expire_Fetch_is_null()
        {
            Thread.Sleep(6000);

            var entity = await _persistedGrantCosmosStore.FindAsync(_currentEntity.Id, _currentEntity.Key);
            entity.Should().BeNull();
        }
    }
}
