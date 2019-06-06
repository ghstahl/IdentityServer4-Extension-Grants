using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Services;
using IdentityServer4Extras.Stores;
using IdentityServer4Extras.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4Extras.Extensions
{
    public static class IdentityServer4Extensions
    {/// <summary>
     /// Adds the in memory clients.
     /// </summary>
     /// <param name="builder">The builder.</param>
     /// <param name="clients">The clients.</param>
     /// <returns></returns>
        public static IIdentityServerBuilder AddInMemoryClientsExtra(this IIdentityServerBuilder builder,
            IEnumerable<Client> clients)
        {
            builder.Services.AddSingleton(clients);
            builder.AddInMemoryClientStoreExtra();

            var existingCors = builder.Services.Where(x => x.ServiceType == typeof(ICorsPolicyService)).LastOrDefault();
            if (existingCors != null &&
                existingCors.ImplementationType == typeof(DefaultCorsPolicyService) &&
                existingCors.Lifetime == ServiceLifetime.Transient)
            {
                // if our default is registered, then overwrite with the InMemoryCorsPolicyService
                // otherwise don't overwrite with the InMemoryCorsPolicyService, which uses the custom one registered by the host
                builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
            }
            return builder;
        }

        public static IIdentityServerBuilder AddIdentityServer4Extras(this IIdentityServerBuilder builder)
        {

            return builder;
        }
        public static IIdentityServerBuilder AddPluginHostClientSecretValidator(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ClientSecretValidator>();
            builder.Services.RemoveAll<IClientSecretValidator>();
            builder.Services.AddTransient<ClientSecretValidator>();
            builder.Services.TryAddTransient<IClientSecretValidator, PluginHostClientSecretValidator>();
            return builder;
        }
        public static IIdentityServerBuilder AddNoSecretRefreshClientSecretValidator(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientSecretValidatorPlugin, NoSecretRefreshClientSecretValidator>();
            return builder;
        }

        public static IIdentityServerBuilder AddInMemoryClientStoreExtra(this IIdentityServerBuilder builder)
        {

            builder.Services.RemoveAll<IClientStore>();
            builder.Services.TryAddTransient<InMemoryClientStoreExtra>();
            builder.Services.TryAddTransient<IClientStoreExtra, InMemoryClientStoreExtra>();
            builder.AddClientStoreCacheExtra<InMemoryClientStoreExtra>();
            return builder;
        }

        public static IIdentityServerBuilder AddNullRefreshTokenKeyObfuscator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IRefreshTokenKeyObfuscator>();
            builder.Services.AddTransient<IRefreshTokenKeyObfuscator, NullRefreshTokenKeyObfuscator>();
            return builder;
        }
        public static IIdentityServerBuilder AddProtectedRefreshTokenKeyObfuscator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IRefreshTokenKeyObfuscator>();
            builder.Services.AddTransient<IRefreshTokenKeyObfuscator, ProtectedRefreshTokenKeyObfuscator>();
            return builder;
        }

        public static IIdentityServerBuilder SwapOutTokenResponseGenerator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenResponseGenerator>();
            builder.Services.TryAddTransient<ITokenResponseGenerator, TokenResponseGeneratorHook>();
            return builder;
        }

        public static IIdentityServerBuilder SwapOutDefaultTokenService(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenService>();
            builder.Services.TryAddTransient<DefaultTokenService>();
            builder.Services.TryAddTransient<ITokenService, DefaultTokenServiceHook>();
            return builder;
        }

        public static IIdentityServerBuilder SwapOutScopeValidator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IScopeValidator>();
            builder.Services.TryAddTransient<IScopeValidator, MyScopeValidator>();
            return builder;
        }
        public static IIdentityServerBuilder SwapOutTokenRevocationRequestValidator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenRevocationRequestValidator>();
            builder.Services.TryAddTransient<ITokenRevocationRequestValidator, SubjectTokenRevocationRequestValidator>();
            return builder;
        }
        public static IIdentityServerBuilder SwapOutDefaultClaimsService(
            this IIdentityServerBuilder builder)
        {

            builder.Services.RemoveAll<IClaimsService>();
            builder.Services.TryAddTransient<DefaultClaimsService>();
            builder.Services.TryAddTransient<IClaimsService, ExtrasClaimsService>();
            return builder;
        }
    }
}
