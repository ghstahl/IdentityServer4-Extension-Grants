// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Logging;
using IdentityServer4.Stores;
using IdentityServer4Extras.Stores;
using System.Collections.Generic;

namespace IdentityServer4Extras.Caching
{
    /// <summary>
    /// Cache decorator for IClientStore
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IClientStoreExtra" />
    public class CachingClientStoreExtra<T> : IClientStore
        where T : IClientStoreExtra
    {
        private readonly IdentityServerOptions _options;
        private readonly ICache<ClientExtra> _cache;
        private readonly IClientStore _inner;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingClientStoreExtra{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="logger">The logger.</param>
        public CachingClientStoreExtra(IdentityServerOptions options, T inner, ICache<ClientExtra> cache, ILogger<CachingClientStoreExtra<T>> logger)
        {
            _options = options;
            _inner = inner;
            _cache = cache;
            _logger = logger;
        }
        async Task<ClientExtra> GetAsync(string clientId)
        {
            var client = await _inner.FindClientByIdAsync(clientId);
            return client as ClientExtra;
        }
        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _cache.GetAsync(clientId,
                _options.Caching.ClientStoreExpiration,
                () => GetAsync(clientId),
                _logger);

            return client;
        }
    }
}