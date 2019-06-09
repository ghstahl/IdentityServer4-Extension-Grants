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
    public static class ApiResourceMapperProfileExtensions
    {
        static ApiResourceMapperProfileExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ApiResource ToModel(this Entities.ApiResourceEntity entity)
        {
            return entity == null ? null : Mapper.Map<ApiResource>(entity);
        }

        public static Entities.ApiResourceEntity ToEntity(this ApiResource model)
        {
            return model == null ? null : Mapper.Map<Entities.ApiResourceEntity>(model);
        }

        public static void UpdateEntity(this ApiResource model, Entities.ApiResourceEntity entity)
        {
            Mapper.Map(model, entity);
        }
    }
}