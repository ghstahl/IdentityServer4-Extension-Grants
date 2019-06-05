using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Contrib.Cosmonaut.Stores
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private ICosmosStore<PersistedGrantEntity> _persistedGrantCosmosStore;
        private ILogger<PersistedGrantStore> _logger;

        public PersistedGrantStore(
            ICosmosStore<PersistedGrantEntity> persistedGrantCosmosStore,
            ILogger<PersistedGrantStore> logger)
        {
            _persistedGrantCosmosStore = persistedGrantCosmosStore;
            _logger = logger;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            Guard.ForNull(grant, nameof(grant));
            Guard.ForNull(grant.CreationTime, nameof(grant.CreationTime));
            Guard.ForNull(grant.Expiration, nameof(grant.Expiration));
            Guard.ForNullOrWhitespace(grant.ClientId, nameof(grant.ClientId));
            Guard.ForNullOrWhitespace(grant.SubjectId, nameof(grant.SubjectId));
            Guard.ForNullOrWhitespace(grant.Data, nameof(grant.Data));
            Guard.ForNullOrWhitespace(grant.Key, nameof(grant.Key));
            Guard.ForNullOrWhitespace(grant.Type, nameof(grant.Type));

            if (grant.Expiration <= grant.CreationTime)
            {
                throw new ArgumentOutOfRangeException();
            }

            var entity = grant.ToEntity();
            var ttl = (int)(entity.Expiration - entity.CreationTime)?.TotalSeconds;

            if (ttl <= 0)
            {
                ttl = -1;
            }
            entity.TTL = ttl;
            var response = await _persistedGrantCosmosStore.UpsertAsync(entity);
            if (!response.IsSuccess)
            {
                _logger.LogCritical("Could not store PersitedGrant");
            }
        }
        public async Task<PersistedGrant> GetAsync(string key)
        {
            Guard.ForNull(key, nameof(key));
            var sql = $"SELECT* FROM c where c.key = \"{key}\"";
            var entity = (await _persistedGrantCosmosStore.QuerySingleAsync(sql, feedOptions: new Microsoft.Azure.Documents.Client.FeedOptions
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
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            Guard.ForNull(subjectId, nameof(subjectId));
            var sql = $"SELECT* FROM c where c.SubjectId = \"{subjectId}\"";
            var persistedGrants = (await _persistedGrantCosmosStore.QueryMultipleAsync(sql)).ToList();
            var model = persistedGrants.Select(x => x.ToModel());

            _logger.LogDebug($"{persistedGrants.Count} persisted grants found for {subjectId}");
            return model;
        }
        public async Task RemoveAsync(string key)
        {
            Guard.ForNull(key, nameof(key));
            var sql = $"SELECT* FROM c where c.key = \"{key}\"";
            var entity = (await _persistedGrantCosmosStore.QuerySingleAsync(sql, feedOptions: new Microsoft.Azure.Documents.Client.FeedOptions
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(key)
            }));
            if (entity == null)
            {
                return;
            }

            await _persistedGrantCosmosStore.RemoveByIdAsync(entity.Id, entity.Key);

        }
        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            //SELECT * FROM c where c.ClientId = "5e1736d9-f434-4316-a495-ca43000a7517" and c.SubjectId = "96553b81-9748-4211-a8af-4cdcd9dcad5f"

            Guard.ForNull(subjectId, nameof(subjectId));
            Guard.ForNull(clientId, nameof(clientId));
            var sql = $"SELECT* FROM c where c.SubjectId = \"{subjectId}\" and c.ClientId = \"{clientId}\"";
            var persistedGrants = (await _persistedGrantCosmosStore.QueryMultipleAsync(sql)).ToList();
            foreach (var entity in persistedGrants)
            {
                await _persistedGrantCosmosStore.RemoveByIdAsync(entity.Id, entity.Key);
            }

        }
        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            Guard.ForNull(subjectId, nameof(subjectId));
            Guard.ForNull(clientId, nameof(clientId));
            Guard.ForNull(type, nameof(type));

            var sql = $"SELECT* FROM c where c.SubjectId = \"{subjectId}\" and c.ClientId = \"{clientId}\" and c.Type = \"{type}\"";
            var persistedGrants = (await _persistedGrantCosmosStore.QueryMultipleAsync(sql)).ToList();
            foreach (var entity in persistedGrants)
            {
                await _persistedGrantCosmosStore.RemoveByIdAsync(entity.Id, entity.Key);
            }
        }
    }
}
