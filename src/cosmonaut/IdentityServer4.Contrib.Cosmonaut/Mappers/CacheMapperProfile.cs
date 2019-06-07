using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Models;

namespace IdentityServer4.Contrib.Cosmonaut.Mappers
{
    /// <inheritdoc />
    /// <summary>
    ///     AutoMapper Config for PersistedGrant
    ///     Between Model and Entity
    ///     <seealso cref="!:https://github.com/AutoMapper/AutoMapper/wiki/Configuration">
    ///     </seealso>
    /// </summary>
    public class CacheMapperProfile : Profile
    {
        /// <summary>
        ///     <see cref="CacheMapperProfile">
        ///     </see>
        /// </summary>
        public CacheMapperProfile()
        {
            // entity to model
            CreateMap<CacheEntity, CacheItem>(MemberList.Destination);

            // model to entity
            CreateMap<CacheItem, CacheEntity>(MemberList.Source);
        }
    }
}
