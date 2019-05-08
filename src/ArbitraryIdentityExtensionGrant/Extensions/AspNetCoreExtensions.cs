using IdentityServer4Extras.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryIdentityExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryIdentityExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryIdentityRequestValidator>();
            services.AddTransient<ITokenResponseGeneratorHook, TokenResponseGeneratorHook>();
            services.AddTransient<ITokenServiceHookPlugin, TokenServiceHookPlugin>();
        }
    }
}