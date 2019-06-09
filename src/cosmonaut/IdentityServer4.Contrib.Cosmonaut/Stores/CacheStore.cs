using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Contrib.Cosmonaut.Interfaces;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Contrib.Cosmonaut.Stores
{
    public class CacheStore : ICacheStore<CacheItem>
    {
        private ICosmosStore<CacheEntity> _store;
        private ILogger<CacheStore> _logger;

        public CacheStore(
            ICosmosStore<CacheEntity> store,
            ILogger<CacheStore> logger)
        {
            _store = store;
            _logger = logger;
        }
        public async Task<bool> SetAsync(string key, CacheItem item, TimeSpan expiration)
        {
            Guard.ForNullOrWhitespace(key, nameof(key));
            Guard.ForNull(item, nameof(item));
            Guard.ForNull(expiration, nameof(expiration));
            var entity = item.ToEntity();
            var ttl = (int)expiration.TotalSeconds;

            if (ttl <= 0)
            {
                ttl = -1;
            }
            entity.TTL = ttl;
            var response = await _store.UpsertAsync(entity);
            if (!response.IsSuccess)
            {
                _logger.LogCritical("Could not store PersitedGrant");
            }
            return response.IsSuccess;

        }
        public async Task<CacheItem> GetAsync(string key)
        {
            Guard.ForNull(key, nameof(key));
            var sql = $"SELECT* FROM c where c.key = \"{key}\"";
            var entity = (await _store.QuerySingleAsync(sql, feedOptions: new Microsoft.Azure.Documents.Client.FeedOptions
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(key)
            }));
            if (entity == null)
            {
                return null;
            }
            var model = entity.ToModel();

            return model;

        }
    }
}
