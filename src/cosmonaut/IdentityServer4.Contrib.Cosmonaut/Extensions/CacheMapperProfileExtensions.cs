using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Mappers;
using IdentityServer4.Contrib.Cosmonaut.Models;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    /// <summary>
    ///     Extension methods to map CacheItem to
    ///     CacheEntity.
    /// </summary>
    public static class CacheMapperProfileExtensions
    {
        static CacheMapperProfileExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<CacheMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static CacheItem ToModel(this Entities.CacheEntity entity)
        {
            return entity == null ? null : Mapper.Map<CacheItem>(entity);
        }

            public static Entities.CacheEntity ToEntity(this CacheItem model)
        {
            return model == null ? null : Mapper.Map<Entities.CacheEntity>(model);
        }

        public static void UpdateEntity(this CacheItem model, Entities.CacheEntity entity)
        {
            Mapper.Map(model, entity);
        }
    }
}