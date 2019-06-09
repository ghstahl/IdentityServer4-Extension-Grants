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
    public class ApiResourceMapperProfile : Profile
    {
        /// <summary>
        ///     <see cref="PersistedGrantMapperProfile">
        ///     </see>
        /// </summary>
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResourceEntity, ApiResource>(MemberList.Destination);

            // model to entity
            CreateMap<ApiResource, ApiResourceEntity>(MemberList.Source);
        }
    }
}
