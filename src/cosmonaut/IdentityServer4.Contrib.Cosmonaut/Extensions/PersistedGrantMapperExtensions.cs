using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Mappers;
using IdentityServer4.Models;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    /// <summary>
    ///     Extension methods to map IdentityServer4.Models.PersistedGrant to
    ///     PersistedGrantEntity.
    /// </summary>
    public static class PersistedGrantMapperExtensions
    {
        static PersistedGrantMapperExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        ///     Will map all data in IdentityServer4.Contrib.CosmosDB.Entities.PersistedGrant to
        ///     IdentityServer4.Models.PersistedGrant.
        /// </summary>
        /// <param name="entity">The IdentityServer4.Contrib.CosmosDB.Entities.PersistedGrant to gather data from.</param>
        /// <returns>An instance of IdentityServer4.Models.PersistedGrant.</returns>
        public static PersistedGrant ToModel(this Entities.PersistedGrantEntity entity)
        {
            return entity == null ? null : Mapper.Map<PersistedGrant>(entity);
        }

        /// <summary>
        ///     Will map all data in IdentityServer4.Models.PersistedGrant to
        ///     IdentityServer4.Contrib.CosmosDB.Entities.PersistedGrant.
        /// </summary>
        /// <param name="model">The IdentityServer4.Models.PersistedGrant to gather data from.</param>
        /// <returns>An instance of IdentityServer4.Contrib.CosmosDB.Entities.PersistedGrant.</returns>
        public static Entities.PersistedGrantEntity ToEntity(this PersistedGrant model)
        {
            return model == null ? null : Mapper.Map<Entities.PersistedGrantEntity>(model);
        }

        /// <summary>
        ///     Will update all data in IdentityServer4.Contrib.CosmosDB.Entities.PersistedGrant with
        ///     IdentityServer4.Models.PersistedGrant.
        /// </summary>
        /// <param name="model">IdentityServer4.Models.PersistedGrant to update from.</param>
        /// <param name="entity">IdentityServer4.Contrib.CosmosDB.Entities.PersistedGrant to update.</param>
        public static void UpdateEntity(this PersistedGrant model, Entities.PersistedGrantEntity entity)
        {
            Mapper.Map(model, entity);
        }
    }
}