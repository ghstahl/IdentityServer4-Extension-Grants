using AutoMapper;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Contrib.Cosmonaut.Mappers
{
    /// <inheritdoc />
    /// <summary>
    ///     AutoMapper Config for ApiResource
    ///     Between Model and Entity
    ///     <seealso cref="!:https://github.com/AutoMapper/AutoMapper/wiki/Configuration">
    ///     </seealso>
    /// </summary>
    public class IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        ///     <see cref="IdentityResourceMapperProfile">
        ///     </see>
        /// </summary>
        public IdentityResourceMapperProfile()
        {
            // entity to model
            CreateMap<IdentityResourceEntity, IdentityResource>(MemberList.Destination);

            // model to entity
            CreateMap<IdentityResource, IdentityResourceEntity>(MemberList.Source);
        }
    }
}
