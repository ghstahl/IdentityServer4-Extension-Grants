// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Caching;
using IdentityServer4Extras.Stores;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Builder extension methods for registering additional services 
    /// </summary>
    public static class IdentityServerBuilderExtensionsAdditionalExtra
    {

        /// <summary>
        /// Adds the client store cache.
        /// </summary>
        /// <typeparam name="T">The type of the concrete client store class that is registered in DI.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddClientStoreCacheExtra<T>(this IIdentityServerBuilder builder)
            where T : IClientStoreExtra
        {
            builder.Services.TryAddTransient(typeof(T));
            builder.Services.AddTransient<ValidatingClientStoreExtra<T>>();
            builder.Services.AddTransient<IClientStore, CachingClientStoreExtra<ValidatingClientStoreExtra<T>>>();

            return builder;
        }
    }
}