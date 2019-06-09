using Cosmonaut;
using IdentityServer4.Configuration;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Contrib.Cosmonaut.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Contrib.Cosmonaut.Stores
{
    public class ResourcesStore : IFullResourceStore
    {
        private IdentityServerOptions _options;
        private IMemoryCache _memoryCache;
        private ICosmosStore<ApiResourceEntity> _apiResourceGrantCosmosStore;
        private ILogger<ResourcesStore> _logger;
        private static string ApiResourcesCacheKey = "66fbd540-4921-44a0-8948-3e1a33711d1e";

        public ResourcesStore(
            IdentityServerOptions options,
            IMemoryCache memoryCache,
            ICosmosStore<ApiResourceEntity> apiResourceGrantCosmosStore,
            ILogger<ResourcesStore> logger)
        {
            _options = options;
            _memoryCache = memoryCache;
            _apiResourceGrantCosmosStore = apiResourceGrantCosmosStore;
            _logger = logger;
        }

        public async Task<IEnumerable<ApiResource>> FetchAllApiResourcesAsync()
        {
            List<ApiResource> currentApiResources;
            bool isExist = _memoryCache.TryGetValue(ApiResourcesCacheKey, out currentApiResources);
            if (!isExist)
            {
                var sql = $"SELECT* FROM c";
                var apiResources = (await _apiResourceGrantCosmosStore.QueryMultipleAsync(sql));
                var query = from item in apiResources
                            select item.ToModel();
                currentApiResources = query.ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_options.Caching.ResourceStoreExpiration);
                _memoryCache.Set(ApiResourcesCacheKey, currentApiResources, cacheEntryOptions);
            }
            return currentApiResources;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            Guard.ForNull(name, nameof(name));
            var sql = $"SELECT* FROM c where c.name = \"{name}\"";
            var entity = (await _apiResourceGrantCosmosStore.QuerySingleAsync(sql, feedOptions: new Microsoft.Azure.Documents.Client.FeedOptions
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(name)
            }));
            if (entity == null)
            {
                return null;
            }
            var model = entity.ToModel();

            return model;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT* FROM c where ARRAY_CONTAINS(c.scopes,");
            var query = from item in scopeNames
                        let c = $"{{name:\"{item}\"}}"
                        select c;

            sb.Append(string.Join(',', query));
            sb.Append(")");
            var sql = sb.ToString();

            //            var sql = $"SELECT* FROM c where c.name = \"{name}\"";
            //where ARRAY_CONTAINS(c.scopes,{name:"scope1"},{name:"scope2"})

            var result = await _apiResourceGrantCosmosStore.QueryMultipleAsync(sql);
            var q2 = from item in result
                     select item.ToModel();
            return q2;
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task StoreAsync(ApiResource model)
        {
            Guard.ForNull(model, nameof(model));
            Guard.ForNull(model.Name, nameof(model.Name));
            var entity = model.ToEntity();
            var response = await _apiResourceGrantCosmosStore.UpsertAsync(entity);
            if (!response.IsSuccess)
            {
                _logger.LogCritical("Could not store PersitedGrant");
            }
        }
        public async Task RemoveApiResourceAsync(string name)
        {
            Guard.ForNull(name, nameof(name));
            var sql = $"SELECT* FROM c where c.name = \"{name}\"";
            var entity = (await _apiResourceGrantCosmosStore.QuerySingleAsync(sql, feedOptions: new Microsoft.Azure.Documents.Client.FeedOptions
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(name)
            }));
            if (entity == null)
            {
                return;
            }

            await _apiResourceGrantCosmosStore.RemoveByIdAsync(entity.Id, entity.Name);

        }
    }
}
