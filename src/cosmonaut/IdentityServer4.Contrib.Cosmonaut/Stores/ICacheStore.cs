using System;
using System.Threading.Tasks;

namespace IdentityServer4.Contrib.Cosmonaut.Stores
{
    public interface ICacheStore<T> where T : class
    {
        /// <summary>
        /// Gets the cached data based upon a key index.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cached item, or <c>null</c> if no item matches the key.</returns>
        Task<T> GetAsync(string key);

        /// <summary>
        /// Caches the data based upon a key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        /// <returns></returns>
        Task<bool> SetAsync(string key, T item, TimeSpan expiration);
    }
}
