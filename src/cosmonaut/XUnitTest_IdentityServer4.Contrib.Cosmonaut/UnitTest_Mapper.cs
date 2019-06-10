using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Contrib.Cosmonaut.Models;
using IdentityServer4.Models;
using Xunit;


namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{
    public class UnitTest_Mapper
    {
        //A method of type bool to give you the result of equality between two lists
        bool CompareLists<T>(List<T> list1, List<T> list2)
        {
            //here we check the count of list elements if they match, it can work also if the list count doesn't meet, to do it just comment out this if statement
            if (list1.Count != list2.Count)
                return false;

            //here we check and find every element from the list1 in the list2
            foreach (var item in list1)
                if (list2.Find(i => i.Equals(item)) == null)
                    return false;

            //here we check and find every element from the list2 in the list1 to make sure they don't have repeated and mismatched elements
            foreach (var item in list2)
                if (list1.Find(i => i.Equals(item)) == null)
                    return false;

            //return true because we didn't find any missing element
            return true;
        }
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
        [Fact]
        public void map_CacheItem_to_Entity()
        {
            var model = new CacheItem
            {
                Key = NewGuidS,
                Data = NewGuidS
            };
            var entity = model.ToEntity();
            entity.Id = NewGuidS;
            entity.TTL = 1;

            var actual = entity.ToModel();
            model.Data.Should().Be(actual.Data);
            model.Key.Should().Be(actual.Key);
        }
        [Fact]
        public void map_Secret_to_Entity()
        {
            var model = new Secret
            {
                Value = NewGuidS,
                Description = NewGuidS,
                Type = NewGuidS,
                Expiration = DateTime.UtcNow
            };
            var entity = model.ToEntity();


            var actual = entity.ToModel();
            model.Value.Should().Be(actual.Value);
            model.Description.Should().Be(actual.Description);
            model.Type.Should().Be(actual.Type);
            model.Expiration.Should().Be(actual.Expiration);
        }
        [Fact]
        public void map_Scope_to_Entity()
        {
            var model = new Scope(NewGuidS, NewGuidS, new List<string> { NewGuidS })
            {
                Description = NewGuidS,
                Emphasize = true,
                Required = true,
                ShowInDiscoveryDocument = true
            };
            var entity = model.ToEntity();


            var actual = entity.ToModel();
            model.Description.Should().Be(actual.Description);
            model.DisplayName.Should().Be(actual.DisplayName);
            model.Emphasize.Should().Be(actual.Emphasize);
            model.Name.Should().Be(actual.Name);
            model.Required.Should().Be(actual.Required);
            model.ShowInDiscoveryDocument.Should().Be(actual.ShowInDiscoveryDocument);
            model.UserClaims.Count.Should().Be(actual.UserClaims.Count);


            var comp = CompareLists(model.UserClaims.ToList(), actual.UserClaims.ToList());
            comp.Should().BeTrue();
        }
        [Fact]
        public void map_ApiResource_to_Entity()
        {
            var model = new ApiResource
            {
                DisplayName = NewGuidS,
                Description = NewGuidS,
                Enabled = true,
                Name = NewGuidS,
                UserClaims = new List<string> { NewGuidS },
                Scopes = new List<Scope> {
                    new Scope(NewGuidS, NewGuidS, new List<string> { NewGuidS })
                    {
                        Description = NewGuidS,
                        Emphasize = true,
                        Required = true,
                        ShowInDiscoveryDocument = true
                    }
                },
                ApiSecrets = new List<Secret> {
                    new Secret
                    {
                        Value = NewGuidS,
                        Description = NewGuidS,
                        Type = NewGuidS,
                        Expiration = DateTime.UtcNow
                    }
                },
                Properties = new Dictionary<string, string>()
                {
                    { NewGuidS,NewGuidS}
                }
            };
            var entity = model.ToEntity();


            var actual = entity.ToModel();

            model.DeepCompare(actual).Should().BeTrue();

        }
        [Fact]
        public void map_IdentityResource_to_Entity()
        {
            var model = new IdentityResource
            {
                DisplayName = NewGuidS,
                Description = NewGuidS,
                Enabled = true,
                Name = NewGuidS,
                UserClaims = new List<string> { NewGuidS },
                Emphasize = true,
                Required = true,
                ShowInDiscoveryDocument = true,
                Properties = new Dictionary<string, string>()
                {
                    { NewGuidS,NewGuidS}
                }
            };
            var entity = model.ToEntity();


            var actual = entity.ToModel();

            model.DeepCompare(actual).Should().BeTrue();

        }
    }
}
