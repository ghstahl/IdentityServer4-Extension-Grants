using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace P7Core.Extensions
{
    public static class AspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddP7Core(this IServiceCollection services)
        {
            services.AddTransient(typeof(LazyService<>));
            return services;
        }
    }
}
