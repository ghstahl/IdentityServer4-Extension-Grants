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
        private ICosmosStore<ApiResourceEntity> _apiResourceGrantCosmosStore;
        private ILogger<ResourcesStore> _logger;
        private static string ApiResourcesCacheKey = "66fbd540-4921-44a0-8948-3e1a33711d1e";

        public ResourcesStore(
            IdentityServerOptions options,
            ICosmosStore<ApiResourceEntity> apiResourceGrantCosmosStore,
            ILogger<ResourcesStore> logger)
        {
            _options = options;
            _apiResourceGrantCosmosStore = apiResourceGrantCosmosStore;
            _logger = logger;
        }

        public async Task<IEnumerable<ApiResource>> FetchAllApiResourcesAsync()
        {
            var sql = $"SELECT* FROM c";
            var apiResources = (await _apiResourceGrantCosmosStore.QueryMultipleAsync(sql));
            var query = from item in apiResources
                        select item.ToModel();

            return query;
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
            /*
             SELECT distinct value c FROM c
            join scope in c.scopes where Array_contains(["apples", "strawberries", "bananas"],scope.name,false)
             */
            const string sqlTemplate = "SELECT distinct value c FROM c join scope in c.scopes where Array_contains([{0}],scope.name,false)";
            var query = from item in scopeNames
                        let c = $"'{item}'"
                        select c;

            var sql = string.Format(sqlTemplate, string.Join(',', query));

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

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiResources = await FetchAllApiResourcesAsync();
            return new Resources
            {
                ApiResources = apiResources.ToList()
            };
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

            var response = await _apiResourceGrantCosmosStore.RemoveByIdAsync(entity.Id, entity.Name);

        }
    }
}
