using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Response;
using FluentAssertions;
using IdentityServer4.Contrib.Cosmonaut.Entities;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Azure.Documents;
using Xunit;
using XUnitHelpers.TestCaseOrdering;

namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{
    [TestCaseOrderer("XUnitHelpers.TestCaseOrdering.PriorityOrderer", "XUnitHelpers")]
    public class UnitTest_PersistedGrantStore
    {
        string NewGuidS => Guid.NewGuid().ToString() + "/a";

        private DatabaseInitializer _databaseInitializer;
        private ICosmonautClient _cosmonautClient;


        private static string _currentId;
        public static readonly string DatabaseId = $"DB{nameof(UnitTest_PersistedGrantStore)}";
        public static readonly string PersistantGrantCollectionName = $"COL{nameof(UnitTest_PersistedGrantStore)}_PersistedGrant";
        private IPersistedGrantStore _persistedGrantStore;
        private static PersistedGrant _currentPersistedGrant;
        private ICosmosStore<PersistedGrantEntity> _persistedGrantCosmosStore;
        private static List<PersistedGrantEntity> _currentManyEntities;
        private static PersistedGrantEntity _currentRemoveAllEntity;
        private static PersistedGrantEntity _currentRemoveAllEntityType;

        public UnitTest_PersistedGrantStore(
            DatabaseInitializer databaseInitializer,
            ICosmonautClient cosmonautClient,
            ICosmosStore<PersistedGrantEntity> persistedGrantCosmosStore,
            IPersistedGrantStore persistedGrantStore)
        {
            _databaseInitializer = databaseInitializer;
            _cosmonautClient = cosmonautClient;
            _persistedGrantCosmosStore = persistedGrantCosmosStore;
            _persistedGrantStore = persistedGrantStore;
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
        [Fact]
        public async Task exception_store_key_null_or_whitespace()
        {
            var ttl = 100; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = null,
                    ClientId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    SubjectId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

            result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = "    ",
                    ClientId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    SubjectId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

        }
        [Fact]
        public async Task exception_store_clientId_null_or_whitespace()
        {
            var ttl = 100; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    ClientId = null,
                    CreationTime = DateTime.UtcNow,
                    SubjectId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

            result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    ClientId = "    ",
                    CreationTime = DateTime.UtcNow,
                    SubjectId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

        }
        [Fact]
        public async Task exception_store_SubjectId_null_or_whitespace()
        {
            var ttl = 100; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = null,
                    CreationTime = DateTime.UtcNow,
                    ClientId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

            result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = "    ",
                    CreationTime = DateTime.UtcNow,
                    ClientId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

        }
        [Fact]
        public async Task exception_store_Data_null_or_whitespace()
        {
            var ttl = 100; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    ClientId = NewGuidS,
                    Data = null,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

            result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    ClientId = NewGuidS,
                    Data = "    ",
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

        }
        [Fact]
        public async Task exception_store_Type_null_or_whitespace()
        {
            var ttl = 100; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    ClientId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = null
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

            result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    ClientId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = "    "
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

        }
        [Fact]
        public async Task exception_store_time_out_of_range()
        {
            var ttl = 100; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = NewGuidS,
                    SubjectId = NewGuidS,
                    CreationTime = DateTime.UtcNow.AddSeconds(ttl),
                    ClientId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow,
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant).GetAwaiter().GetResult();

            });

            //Assert
            result.Should().Throw<Exception>();

        }
        [Fact, TestPriority(0)]
        public async Task Persist_Grant_Success()
        {
            var ttl = 4; // 2 seconds
            // Act
            var result = new Action(() =>
            {
                _currentId = NewGuidS;
                _currentPersistedGrant = new PersistedGrant
                {
                    Key = _currentId,
                    ClientId = NewGuidS,
                    CreationTime = DateTime.UtcNow,
                    SubjectId = NewGuidS,
                    Data = NewGuidS,
                    Expiration = DateTime.UtcNow.AddSeconds(ttl),
                    Type = NewGuidS
                };

                _persistedGrantStore.StoreAsync(_currentPersistedGrant);

            });

            //Assert
            result.Should().NotThrow();

        }
        [Fact, TestPriority(1)]
        public async Task Persist_Grant_Get()
        {
            var persistedGrant = await _persistedGrantStore.GetAsync(_currentId);

            persistedGrant.Should().NotBeNull();

            persistedGrant.ClientId.Should().Be(_currentPersistedGrant.ClientId);
            persistedGrant.CreationTime.Should().Be(_currentPersistedGrant.CreationTime);
            persistedGrant.Data.Should().Be(_currentPersistedGrant.Data);
            persistedGrant.Expiration.Should().Be(_currentPersistedGrant.Expiration);
            persistedGrant.Key.Should().Be(_currentPersistedGrant.Key);
            persistedGrant.SubjectId.Should().Be(_currentPersistedGrant.SubjectId);
            persistedGrant.Type.Should().Be(_currentPersistedGrant.Type);

            Thread.Sleep(5000);

            persistedGrant = await _persistedGrantStore.GetAsync(_currentId);

            persistedGrant.Should().BeNull();
        }
        [Fact, TestPriority(-1)]
        public async Task add_many_PersistedGrantEntity()
        {
            _currentManyEntities = new List<PersistedGrantEntity>();

            for (int i = 0; i < 2; i++)
            {
                var clientId = NewGuidS;
                var subjectId = NewGuidS;
                var ttl = 60 * 60;
                for (var j = 0; j < 10; j++)
                {
                    _currentManyEntities.Add(new PersistedGrantEntity
                    {
                        Key = NewGuidS,
                        ClientId = clientId,
                        CreationTime = DateTime.UtcNow,
                        SubjectId = subjectId,
                        Data = NewGuidS,
                        Expiration = DateTime.UtcNow.AddSeconds(ttl),
                        Type = NewGuidS,
                        TTL = ttl
                    });
                }
            }

            var id = NewGuidS;


            var addedResults = await _persistedGrantCosmosStore.AddRangeAsync(_currentManyEntities);

            addedResults.IsSuccess.Should().BeTrue();

            addedResults.SuccessfulEntities.Count.Should().Be(20);
        }
        [Fact, TestPriority(0)]
        public async Task get_subjects_Success()
        {
            var subjectId = _currentManyEntities[0].SubjectId;

            var entities = await _persistedGrantStore.GetAllAsync(subjectId);
            entities.Count().Should().Be(10);
        }
        [Fact, TestPriority(1)]
        public async Task get_and_remove()
        {
            var key = _currentManyEntities[0].Key;

            var entity = await _persistedGrantStore.GetAsync(key);
            entity.Data.Should().Be(_currentManyEntities[0].Data);
            await _persistedGrantStore.RemoveAsync(key);
        }
        [Fact, TestPriority(2)]
        public async Task get_not_found_after_remove()
        {
            var key = _currentManyEntities[0].Key;
            var entity = await _persistedGrantStore.GetAsync(key);
            entity.Should().BeNull();
        }
        [Fact, TestPriority(3)]
        public async Task remove_all_subjectid_clientid()
        {
            _currentRemoveAllEntity = _currentManyEntities[0];
            await _persistedGrantStore.RemoveAllAsync(_currentRemoveAllEntity.SubjectId, _currentRemoveAllEntity.ClientId);
        }
        [Fact, TestPriority(4)]
        public async Task get_not_found_after_remove_all()
        {
            var key = _currentRemoveAllEntity.Key;
            var entity = await _persistedGrantStore.GetAsync(key);
            entity.Should().BeNull();
        }
        [Fact, TestPriority(5)]
        public async Task remove_all_subjectid_clientid_type()
        {
            _currentRemoveAllEntityType = _currentManyEntities[0];
            await _persistedGrantStore.RemoveAllAsync(_currentRemoveAllEntityType.SubjectId, _currentRemoveAllEntityType.ClientId, _currentRemoveAllEntityType.Type);
        }
        [Fact, TestPriority(6)]
        public async Task get_not_found_after_remove_all_type()
        {
            var key = _currentRemoveAllEntityType.Key;
            var entity = await _persistedGrantStore.GetAsync(key);
            entity.Should().BeNull();
        }
    }
}
