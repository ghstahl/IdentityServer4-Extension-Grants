using Cosmonaut;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Contrib.Cosmonaut.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using XUnitHelpers.TestCaseOrdering;

namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{
    [TestCaseOrderer("XUnitHelpers.TestCaseOrdering.PriorityOrderer", "XUnitHelpers")]
    public class UnitTest_ResourceStore
    {
        string NewGuidS => Guid.NewGuid().ToString() + "/a";

        private DatabaseInitializer<UnitTest_ResourceStore> _databaseInitializer;
        private ICosmonautClient _cosmonautClient;
        private ICosmosStore<ApiResourceEntity> _apiResourceCosmosStore;
        private IFullResourceStore _resourceStore;
        private static string _currentId;
        private static ApiResource _currentApiResource;
        public static readonly string DatabaseId = $"DB{nameof(UnitTest_ResourceStore)}";
        public static readonly string CollectionName = $"COL{nameof(UnitTest_ResourceStore)}_Resources";


        public UnitTest_ResourceStore(
            DatabaseInitializer<UnitTest_ResourceStore> databaseInitializer,
            ICosmonautClient cosmonautClient,
            ICosmosStore<ApiResourceEntity> apiResourceCosmosStore,
            IFullResourceStore resourceStore)
        {
            _databaseInitializer = databaseInitializer;
            _cosmonautClient = cosmonautClient;
            _apiResourceCosmosStore = apiResourceCosmosStore;
            _resourceStore = resourceStore;
        }
        [Fact, TestPriority(-1000)]

        public async Task Ensure_Database_ScalingSettings()
        {


            // Act
            var result = new Action(() =>
            {
                _databaseInitializer.action();
            });

            //Assert
            result.Should().NotThrow();

        }

        [Fact, TestPriority(0)]
        public async Task store_apiResource_Success()
        {
            var ttl = 4; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentApiResource = new ApiResource
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

                _resourceStore.StoreAsync(_currentApiResource).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().NotThrow();

        }
        [Fact, TestPriority(2)]
        public async Task get_apiResource_Success()
        {
            var model = await _resourceStore.FindApiResourceAsync(_currentApiResource.Name);
            model.Should().NotBeNull();
            model.DeepCompare(_currentApiResource).Should().BeTrue();
        }
        [Fact, TestPriority(2)]
        public async Task remove_apiResource_Success()
        {
            // Act
            var result = new Action(() =>
            {
                _resourceStore.RemoveApiResourceAsync(_currentApiResource.Name).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().NotThrow();

        }
    }
}
