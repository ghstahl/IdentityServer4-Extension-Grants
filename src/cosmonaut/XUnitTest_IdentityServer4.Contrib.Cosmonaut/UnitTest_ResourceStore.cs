using Cosmonaut;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using IdentityServer4.Contrib.Cosmonaut.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static List<ApiResource> _currenMany;
        private static ApiResource _currentApiResource;
        private static IdentityResource _currentIdentityResource;
        public static readonly string DatabaseId = $"DB{nameof(UnitTest_ResourceStore)}";
        public static readonly string CollectionNameApiResources = $"COL{nameof(UnitTest_ResourceStore)}_ApiResources";
        public static readonly string CollectionNameIdentityResources = $"COL{nameof(UnitTest_ResourceStore)}_IdentityResources";


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
        public async Task store_many_apiResource_Success()
        {
            // Act
            var result = new Action(() =>
            {
                _currenMany = new List<ApiResource>();

                for (int i = 0; i < 10; i++)
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
                    _currenMany.Add(model);
                }
                foreach (var item in _currenMany)
                {
                    _resourceStore.StoreAsync(item).GetAwaiter().GetResult();
                }
            });

            //Assert
            result.Should().NotThrow();

        }
        [Fact, TestPriority(1)]
        public async Task find_many_apiResource_Success()
        {
            var resources = await _resourceStore.GetAllResourcesAsync();
            resources.Should().NotBeNull();
            resources.ApiResources.Any().Should().BeTrue();
        }

        [Fact, TestPriority(2)]
        public async Task remove_many_apiResource_Success()
        {
            // Act
            var result = new Action(() =>
            {


                foreach (var item in _currenMany)
                {
                    _resourceStore.RemoveApiResourceAsync(item.Name).GetAwaiter().GetResult();
                }
            });

            //Assert
            result.Should().NotThrow();

        }
        [Fact, TestPriority(0)]
        public async Task store_apiResource_Success()
        {
            // Act
            var result = new Action(() =>
            {

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
        [Fact, TestPriority(0)]
        public async Task store_identityResource_Success()
        {
            // Act
            var result = new Action(() =>
            {
                _currentIdentityResource = new IdentityResource
                {
                    DisplayName = NewGuidS,
                    Description = NewGuidS,
                    Enabled = true,
                    Name = NewGuidS,
                    UserClaims = new List<string> { NewGuidS },
                    Emphasize = true,
                    Required = true,
                    ShowInDiscoveryDocument = true,
                    Properties = new Dictionary<string, string>(){
                            { NewGuidS,NewGuidS}
                        }
                };

           

                _resourceStore.StoreAsync(_currentIdentityResource).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().NotThrow();

        }
        [Fact, TestPriority(1)]
        public async Task get_apiResource_Success()
        {
            var model = await _resourceStore.FindApiResourceAsync(_currentApiResource.Name);
            model.Should().NotBeNull();
            model.DeepCompare(_currentApiResource).Should().BeTrue();
        }

        [Fact, TestPriority(1)]
        public async Task FindApiResourcesByScopeAsync_Success()
        {
            var scopes = new List<string>
            {
                _currentApiResource.Scopes.FirstOrDefault().Name
            };
            var result = await _resourceStore.FindApiResourcesByScopeAsync(scopes);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);

            result.FirstOrDefault().DeepCompare(_currentApiResource).Should().BeTrue();
        }
        [Fact, TestPriority(1)]
        public async Task FindIdentityesourcesByScopeAsync_Success()
        {
            var scopes = new List<string>
            {
                _currentIdentityResource.Name
            };
            var result = await _resourceStore.FindIdentityResourcesByScopeAsync(scopes);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);

            result.FirstOrDefault().DeepCompare(_currentIdentityResource).Should().BeTrue();
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
        [Fact, TestPriority(2)]
        public async Task remove_identityResource_Success()
        {
            // Act
            var result = new Action(() =>
            {
                _resourceStore.RemoveIdentityResourceAsync(_currentIdentityResource.Name).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().NotThrow();

        }
        [Fact, TestPriority(3)]
        public async Task get_apiResource_notfound()
        {
            var model = await _resourceStore.FindApiResourceAsync(_currentApiResource.Name);
            model.Should().BeNull();
        }
        [Fact, TestPriority(3)]
        public async Task get_identityResource_notfound()
        {
            var model = await _resourceStore.FindIdentityResourceAsync(_currentApiResource.Name);
            model.Should().BeNull();
        }

    }
}
