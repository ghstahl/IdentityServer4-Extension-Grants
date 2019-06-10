using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Mappers;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Models;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    /// <summary>
    ///     Extension methods to map ApiResource to
    ///     ApiResourceEntity.
    /// </summary>
    public static class IdentityResourceMapperProfileExtensions
    {
        static IdentityResourceMapperProfileExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static IdentityResource ToModel(this Entities.IdentityResourceEntity entity)
        {
            return entity == null ? null : Mapper.Map<IdentityResource>(entity);
        }

        public static Entities.IdentityResourceEntity ToEntity(this IdentityResource model)
        {
            return model == null ? null : Mapper.Map<Entities.IdentityResourceEntity>(model);
        }

        public static void UpdateEntity(this IdentityResource model, Entities.IdentityResourceEntity entity)
        {
            Mapper.Map(model, entity);
        }
    }
}