using Cosmonaut;
using IdentityServer4.Contrib.Cosmonaut.Interfaces;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Contrib.Cosmonaut.Cache
{
    /// <summary>
    /// Redis based implementation for ICache<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CosmonautCache<T> : ICache<T> where T : class
    {

        private ICacheStore<CacheItem> _cacheStore;
        private ILogger<CosmonautCache<T>> _logger;

        public CosmonautCache(
           ICacheStore<CacheItem> cacheStore,
            ILogger<CosmonautCache<T>> logger)
        {
            _cacheStore = cacheStore;
            _logger = logger;
        }
        public async Task<T> GetAsync(string key)
        {
            var cacheItem = await _cacheStore.GetAsync(key);
            if (cacheItem == null) return null;
            return Deserialize(cacheItem.Data);
        }

        public async Task SetAsync(string key, T item, TimeSpan expiration)
        {
            var cacheItem = new CacheItem()
            {
                Key = key,
                Data = Serialize(item)
            };
            await _cacheStore.SetAsync(key, cacheItem, expiration);
        }

        #region Json
        private JsonSerializerSettings SerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new ClaimConverter());
                return settings;
            }
        }



        private T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, this.SerializerSettings);
        }

        private string Serialize(T item)
        {
            return JsonConvert.SerializeObject(item, this.SerializerSettings);
        }
        #endregion
    }
}
