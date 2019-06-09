using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Mappers;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Models;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    /// <summary>
    ///     Extension methods to map Secret to
    ///     SecretEntity.
    /// </summary>
    public static class SecretMapperProfileExtensions
    {
        static SecretMapperProfileExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<SecretMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Secret ToModel(this Entities.SecretEntity entity)
        {
            return entity == null ? null : Mapper.Map<Secret>(entity);
        }

        public static Entities.SecretEntity ToEntity(this Secret model)
        {
            return model == null ? null : Mapper.Map<Entities.SecretEntity>(model);
        }

        public static void UpdateEntity(this Secret model, Entities.SecretEntity entity)
        {
            Mapper.Map(model, entity);
        }
    }
}