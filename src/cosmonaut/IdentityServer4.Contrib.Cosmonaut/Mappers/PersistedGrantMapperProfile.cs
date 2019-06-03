using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Entities;
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
    public class PersistedGrantMapperProfile : Profile
    {
        /// <summary>
        ///     <see cref="PersistedGrantMapperProfile">
        ///     </see>
        /// </summary>
        public PersistedGrantMapperProfile()
        {
            // entity to model
            CreateMap<PersistedGrantEntity, Models.PersistedGrant>(MemberList.Destination);

            // model to entity
            CreateMap<Models.PersistedGrant, PersistedGrantEntity>(MemberList.Source);
        }
    }
}
