using System;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using IdentityServer4.Contrib.Cosmonaut.Extensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Demystifier;
using Xunit.DependencyInjection.Logging;

[assembly: TestFramework("XUnitTest_IdentityServer4.Contrib.Cosmonaut.Startup", "XUnitTest_IdentityServer4.Contrib.Cosmonaut")]
// Set the orderer
[assembly: TestCollectionOrderer("XUnitHelpers.TestCaseOrdering.PriorityOrderer", "XUnitHelpers")]

// Need to turn off test parallelization so we can validate the run order
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace XUnitTest_IdentityServer4.Contrib.Cosmonaut
{

    public class DatabaseInitializer<T> where T : class
    {
        private bool Initialized { get; set; }
        public Action action { get; set; }

    }
    public class Startup : DependencyInjectionTestFramework
    {
        public static bool Initialized = false;
        private readonly ConnectionPolicy _connectionPolicy = new ConnectionPolicy
        {
            ConnectionProtocol = Protocol.Tcp,
            ConnectionMode = ConnectionMode.Direct
        };

        private readonly string _emulatorUri = Environment.GetEnvironmentVariable("CosmosDBEndpoint") ?? "https://localhost:8081";
        private readonly string _emulatorKey =
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public Startup(IMessageSink messageSink) : base(messageSink) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer();
            services.AddSingleton<IAsyncExceptionFilter, DemystifyExceptionFilter>();
            services.AddSingleton<DatabaseInitializer<UnitTest_CacheItemCosmosStore>>(sp =>
            {
                return new DatabaseInitializer<UnitTest_CacheItemCosmosStore>()
                {
                    action = () =>
                    {
                        lock (this)
                        {
                            if (!Initialized)
                            {
                                var client = sp.GetRequiredService<ICosmonautClient>();

                                var collection = client.GetCollectionAsync(UnitTest_CacheItemCosmosStore.DatabaseId,
                                  UnitTest_CacheItemCosmosStore.CollectionName).GetAwaiter().GetResult();
                                collection.DefaultTimeToLive = 7700000;
                                var response = client.UpdateCollectionAsync(UnitTest_CacheItemCosmosStore.DatabaseId,
                                    UnitTest_CacheItemCosmosStore.CollectionName, collection)
                                   .GetAwaiter()
                                   .GetResult();

                                Initialized = true;
                            }

                        }

                    }
                };
            });
            services.AddSingleton<DatabaseInitializer<UnitTest_PersistedGrantStore>>(sp =>
            {
                return new DatabaseInitializer<UnitTest_PersistedGrantStore>()
                {
                    action = () =>
                    {
                        lock (this)
                        {
                            if (!Initialized)
                            {
                                var client = sp.GetRequiredService<ICosmonautClient>();

                                var collection = client.GetCollectionAsync(UnitTest_PersistedGrantStore.DatabaseId,
                                  UnitTest_PersistedGrantStore.CollectionName).GetAwaiter().GetResult();
                                collection.DefaultTimeToLive = 7700000;
                                var response = client.UpdateCollectionAsync(UnitTest_PersistedGrantStore.DatabaseId,
                                    UnitTest_PersistedGrantStore.CollectionName, collection)
                                   .GetAwaiter()
                                   .GetResult();

                                Initialized = true;
                            }

                        }

                    }
                };
            });
            services.AddTransient<ICosmonautClient>(sp =>
            {
                var client = new CosmonautClient(_emulatorUri, _emulatorKey, _connectionPolicy);
                return client;
            });

            //   _cosmonautClient = new CosmonautClient(_emulatorUri, _emulatorKey, _connectionPolicy);
            var cosmosStoreSettings = new CosmosStoreSettings(
                UnitTest_PersistedGrantStore.DatabaseId,
                _emulatorUri,
                _emulatorKey,
                s =>
                {
                    s.ConnectionPolicy = _connectionPolicy;
                    s.IndexingPolicy = new IndexingPolicy(
                        new RangeIndex(DataType.Number, -1),
                        new RangeIndex(DataType.String, -1));
                });
            var cosmosStoreSettingsCache = new CosmosStoreSettings(
               UnitTest_CacheItemCosmosStore.DatabaseId,
               _emulatorUri,
               _emulatorKey,
               s =>
               {
                   s.ConnectionPolicy = _connectionPolicy;
                   s.IndexingPolicy = new IndexingPolicy(
                       new RangeIndex(DataType.Number, -1),
                       new RangeIndex(DataType.String, -1));
               });

            builder.AddOperationalStore(cosmosStoreSettings, UnitTest_PersistedGrantStore.CollectionName);
            builder.AddCosmonautIdentityServerCacheStore(cosmosStoreSettingsCache, UnitTest_CacheItemCosmosStore.CollectionName);

        }
        protected override void Configure(IServiceProvider provider)
        {
            provider.GetRequiredService<ILoggerFactory>()
            .AddProvider(new XunitTestOutputLoggerProvider(provider.GetRequiredService<ITestOutputHelperAccessor>()));
        }
    }
}
