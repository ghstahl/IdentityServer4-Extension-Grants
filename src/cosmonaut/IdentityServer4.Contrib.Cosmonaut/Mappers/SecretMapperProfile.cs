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
    ///     AutoMapper Config for Scope
    ///     Between Model and Entity
    ///     <seealso cref="!:https://github.com/AutoMapper/AutoMapper/wiki/Configuration">
    ///     </seealso>
    /// </summary>
    public class SecretMapperProfile : Profile
    {
        /// <summary>
        ///     <see cref="SecretMapperProfile">
        ///     </see>
        /// </summary>
        public SecretMapperProfile()
        {
            // entity to model
            CreateMap<SecretEntity, Secret>(MemberList.Destination);

            // model to entity
            CreateMap<Secret, SecretEntity>(MemberList.Source);
        }
    }
}
