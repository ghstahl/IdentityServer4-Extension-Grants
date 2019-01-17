﻿using System;
using ArbitraryIdentityExtensionGrant.Extensions;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using IdentityModelExtras;
using IdentityModelExtras.Extensions;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7Core;
using P7Core.ObjectContainers.Extensions;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;

namespace IdentityServer4ExtensionGrants.Rollup.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddExtensionGrantsRollup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddObjectContainer();  // use this vs a static to cache class data.
            var clients = configuration.LoadClientsFromSettings();
            var apiResources = configuration.LoadApiResourcesFromSettings();
            var identityResources = configuration.LoadIdentityResourcesFromSettings();

            bool useRedis = Convert.ToBoolean(configuration["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(configuration["appOptions:keyVault:useKeyVault"]);
            bool useKeyVaultSigning = Convert.ToBoolean(configuration["appOptions:keyVault:useKeyVaultSigning"]);

            var builder = services
               .AddIdentityServer(options => { options.InputLengthRestrictions.RefreshToken = 256; })
               .AddInMemoryIdentityResources(identityResources)
               .AddInMemoryApiResources(apiResources)
               .AddInMemoryClientsExtra(clients)
               .AddIdentityServer4Extras()
               .AddProfileServiceManager()
               .AddArbitraryOwnerResourceExtensionGrant()
               .AddArbitraryIdentityExtensionGrant()
               .AddArbitraryNoSubjectExtensionGrant();
            // My Replacement Services.
            if (useRedis)
            {
                var redisConnectionString = configuration["appOptions:redis:redisConnectionString"];
                builder.AddOperationalStore(options =>
                {
                    options.RedisConnectionString = redisConnectionString;
                    options.Db = 1;
                })
                    .AddRedisCaching(options =>
                    {
                        options.RedisConnectionString = redisConnectionString;
                        options.KeyPrefix = "prefix";
                    });

                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });
            }
            else
            {
                builder.AddInMemoryPersistedGrants();
                services.AddDistributedMemoryCache();
            }
            if (useKeyVault)
            {
                builder.AddKeyVaultCredentialStore();
                services.AddKeyVaultTokenCreateServiceTypes();
                services.AddKeyVaultTokenCreateServiceConfiguration(configuration);
                if (useKeyVaultSigning)
                {
                    // this signs the token using azure keyvault to do the actual signing
                    builder.AddKeyVaultTokenCreateService();
                }
            }
            else
            {
                builder.AddDeveloperSigningCredential();
            }

            // my replacement services.
            builder.AddRefreshTokenRevokationGeneratorWorkAround();

            builder.AddPluginHostClientSecretValidator();
            builder.AddNoSecretRefreshClientSecretValidator();

            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra
            builder.SwapOutTokenResponseGenerator();
            builder.SwapOutDefaultTokenService();
            builder.SwapOutScopeValidator();
            builder.SwapOutTokenRevocationRequestValidator();

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryIdentityExtentionGrantTypes();
            services.AddIdentityModelExtrasTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            builder.AddProtectedRefreshTokenKeyObfuscator();
            // Request Tracker
            services.AddIdentityServerRequestTrackerMiddleware();

            // my configurations
            services.RegisterP7CoreConfigurationServices(configuration);

        }
    }
}