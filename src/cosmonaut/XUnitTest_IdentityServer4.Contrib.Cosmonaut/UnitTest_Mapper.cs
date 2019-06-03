using System;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Models;
using Xunit;

namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{
    public class UnitTest_Mapper
    {
        private string NewGuidS => Guid.NewGuid().ToString();
        [Fact]
        public void map_PersistedGrant_to_Entity()
        {
            var persistedGrant = new PersistedGrant()
            {
                ClientId = NewGuidS,
                CreationTime = DateTime.UtcNow,
                Data = NewGuidS,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                Key = NewGuidS,
                SubjectId = NewGuidS,
                Type = NewGuidS
            };
            var entity = persistedGrant.ToEntity();
            entity.Etag = NewGuidS;
            entity.TTL = 20;
            
            var actual = entity.ToModel();

            persistedGrant.ClientId.Should().Be(actual.ClientId);
            persistedGrant.CreationTime.Should().Be(actual.CreationTime);
            persistedGrant.Data.Should().Be(actual.Data);
            persistedGrant.Expiration.Should().Be(actual.Expiration);
            persistedGrant.Key.Should().Be(actual.Key);
            persistedGrant.SubjectId.Should().Be(actual.SubjectId);
            persistedGrant.Type.Should().Be(actual.Type);
     

        }
    }
}
