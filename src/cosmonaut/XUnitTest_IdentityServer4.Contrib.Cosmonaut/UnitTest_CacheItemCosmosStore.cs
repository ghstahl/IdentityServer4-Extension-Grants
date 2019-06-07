using Cosmonaut;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Contrib.Cosmonaut.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using XUnitHelpers.TestCaseOrdering;

namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{

    [TestCaseOrderer("XUnitHelpers.TestCaseOrdering.PriorityOrderer", "XUnitHelpers")]
    public class UnitTest_CacheItemCosmosStore
    {
        string NewGuidS => Guid.NewGuid().ToString() + "/a";

        private DatabaseInitializer<UnitTest_CacheItemCosmosStore> _databaseInitializer;
        private ICosmosStore<CacheEntity> _cosmosStore;
        private ICosmonautClient _cosmonautClient;
        private ICacheStore<CacheItem> _cacheStore;
        private static string _currentId;
        private static CacheItem _currentCacheItem;
        public static readonly string DatabaseId = $"DB{nameof(UnitTest_CacheItemCosmosStore)}";
        public static readonly string CollectionName = $"COL{nameof(UnitTest_CacheItemCosmosStore)}_Cache";

        public UnitTest_CacheItemCosmosStore(
            DatabaseInitializer<UnitTest_CacheItemCosmosStore> databaseInitializer,
            ICosmonautClient cosmonautClient,
            ICosmosStore<CacheEntity> cosmosStore,
            ICacheStore<CacheItem> cacheStore)
        {
            _databaseInitializer = databaseInitializer;
            _cosmosStore = cosmosStore;
            _cosmonautClient = cosmonautClient;
            _cacheStore = cacheStore;

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
        public async Task Persist_Cache_That_Will_Expire_in_5_seconds()
        {
            _currentCacheItem = new CacheItem
            {
                Key = NewGuidS,
                Data = NewGuidS,
            };

            var response = await _cacheStore.SetAsync(
                _currentCacheItem.Key,
                _currentCacheItem, new TimeSpan(0, 0, 5));


            response.Should().BeTrue();
        }
        [Fact, TestPriority(1)]
        public async Task Read_cache_success()
        {
            var item = await _cacheStore.GetAsync(_currentCacheItem.Key);

            item.Should().NotBeNull();
            item.Key.Should().Be(_currentCacheItem.Key);
            item.Data.Should().Be(_currentCacheItem.Data);
        }
        [Fact, TestPriority(2)]
        public async Task Read_cache_That_expired()
        {
            Thread.Sleep(6000);
            var item = await _cacheStore.GetAsync(_currentCacheItem.Key);
            item.Should().BeNull();

        }
    }
}
