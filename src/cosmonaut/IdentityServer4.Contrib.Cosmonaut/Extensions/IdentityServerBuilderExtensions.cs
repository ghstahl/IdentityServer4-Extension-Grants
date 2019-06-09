using System;
using System.Collections.Generic;
using System.Text;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using IdentityServer4.Contrib.Cosmonaut.Cache;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Interfaces;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Contrib.Cosmonaut.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    /// <summary>
    ///     Extension methods to setup and configure IdentityServerBuilder
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddCosmonautResourceStore(
                 this IIdentityServerBuilder builder,
                 CosmosStoreSettings settings,
                 string overriddenCollectionName = "")
        {
            builder.Services.AddTransient<IResourceStore, ResourcesStore>();
            builder.Services.AddTransient<IFullResourceStore, ResourcesStore>();
            builder.Services.AddCosmosStore<ApiResourceEntity>(settings, overriddenCollectionName);
            return builder;
        }

        public static IIdentityServerBuilder AddCosmonautIdentityServerCacheStore(
               this IIdentityServerBuilder builder,
               CosmosStoreSettings settings,
               string overriddenCollectionName = "")
        {
            builder.Services.AddIdentityServerCacheCosmonautStore(settings, overriddenCollectionName);
            builder.Services.AddTransient(typeof(ICache<>), typeof(CosmonautCache<>));
            return builder;
        }

        private static IServiceCollection AddIdentityServerCacheCosmonautStore(
               this IServiceCollection services,
               CosmosStoreSettings settings,
               string overriddenCollectionName = "")
        {
            services.AddTransient<ICacheStore<CacheItem>, CacheStore>();

            services.AddCosmosStore<CacheEntity>(settings, overriddenCollectionName);
            return services;
        }
        private static IServiceCollection AddPersistedGrantCosmonautStore(
            this IServiceCollection services,
            CosmosStoreSettings settings,
            string overriddenCollectionName = "")
        {
            services.AddCosmosStore<PersistedGrantEntity>(settings, overriddenCollectionName);
            return services;
        }
        /// <summary>
        ///     Add Operational Store
        /// </summary>
        /// <param name="builder">The IIdentity Server Builder</param>
        /// <param name="setupAction"></param>
        /// <param name="tokenCleanUpOptions"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            CosmosStoreSettings settings,
            string overriddenCollectionName = "")
        {
            builder.Services.AddPersistedGrantCosmonautStore(settings, overriddenCollectionName);
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            return builder;
        }
    }
}
