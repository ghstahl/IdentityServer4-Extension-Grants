using IdentityServer4Extras.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryResourceOwnerExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryResourceOwnerExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryResourceOwnerRequestValidator>();
            services.AddTransient<ITokenServiceHookPlugin, TokenServiceHookPlugin>();
        }
    }
}