using System;
using Microsoft.Extensions.DependencyInjection;

namespace P7Core.Extensions
{
    public static class LazyServiceCollectionExtensions
    {
        public static IServiceCollection AddLazyTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TImplementation>();
            services.AddTransient<Lazy<TService>>(serviceProvider =>
            {

                return new Lazy<TService>(() =>
                {
                    var impl = serviceProvider.GetRequiredService<TImplementation>();
                    return impl;
                });
            });
            return services;
        }
        public static IServiceCollection AddLazySingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddSingleton<TImplementation>();
            services.AddSingleton<Lazy<TService>>(serviceProvider =>
            {

                return new Lazy<TService>(() =>
                {
                    var impl = serviceProvider.GetRequiredService<TImplementation>();
                    return impl;
                });
            });
            return services;
        }
        public static IServiceCollection AddLazyScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddScoped<TImplementation>();
            services.AddScoped<Lazy<TService>>(serviceProvider =>
            {

                return new Lazy<TService>(() =>
                {
                    var impl = serviceProvider.GetRequiredService<TImplementation>();
                    return impl;
                });
            });
            return services;
        }

    }
}
