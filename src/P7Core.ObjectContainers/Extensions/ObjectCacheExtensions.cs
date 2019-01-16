using Microsoft.Extensions.DependencyInjection;

namespace P7Core.ObjectContainers.Extensions
{
    public static class ObjectCacheExtensions
    {
        public static void AddObjectContainer(this IServiceCollection services)
        {
            services.AddSingletonObjectContainer();
            services.AddScopedObjectContainer();
            services.AddAutoObjectAllocator();
        }
        public static void AddSingletonObjectContainer(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ISingletonObjectContainer<,>), typeof(ObjectContainer<,>));
            services.AddSingleton(typeof(ISingletonAutoObjectContainer<,>), typeof(AutoObjectContainer<,>));
        }

        public static void AddScopedObjectContainer(this IServiceCollection services)
        {
            services.AddScoped(typeof(IScopedObjectContainer<,>), typeof(ObjectContainer<,>));
            services.AddScoped(typeof(IScopedAutoObjectContainer<,>), typeof(AutoObjectContainer<,>));
        }
        public static void AddAutoObjectAllocator(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IAutoObjectAllocator<,>), typeof(ObjectAllocator<,>));
        }
    }
}
