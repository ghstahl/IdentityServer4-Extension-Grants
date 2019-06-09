using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Mappers;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Models;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    /// <summary>
    ///     Extension methods to map Scope to
    ///     ScopeEntity.
    /// </summary>
    public static class ScopeMapperProfileExtensions
    {
        static ScopeMapperProfileExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ScopeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Scope ToModel(this Entities.ScopeEntity entity)
        {
            return entity == null ? null : Mapper.Map<Scope>(entity);
        }

        public static Entities.ScopeEntity ToEntity(this Scope model)
        {
            return model == null ? null : Mapper.Map<Entities.ScopeEntity>(model);
        }

        public static void UpdateEntity(this Scope model, Entities.ScopeEntity entity)
        {
            Mapper.Map(model, entity);
        }
    }
}