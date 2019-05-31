using System;
using System.Collections.Generic;
using IdentityServer4.Contrib.CosmosDB.Configuration;
using IdentityServer4.Contrib.CosmosDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Demystifier;
using Xunit.DependencyInjection.Logging;

[assembly: TestFramework("XUnitTest_CosmosDBPersistedGrantStore.Startup", "XUnitTest_CosmosDBPersistedGrantStore")]
// Set the orderer
[assembly: TestCollectionOrderer("XUnitTest_CosmosDBPersistedGrantStore.TestCaseOrdering.PriorityOrderer", "XUnitTest_CosmosDBPersistedGrantStore")]

// Need to turn off test parallelization so we can validate the run order
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace XUnitTest_CosmosDBPersistedGrantStore
{
    public class Startup : DependencyInjectionTestFramework
    {
        public Startup(IMessageSink messageSink) : base(messageSink) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDependency, DependencyClass>();
            services.AddSingleton<IAsyncExceptionFilter, DemystifyExceptionFilter>();
            var builder = services.AddIdentityServer();
            builder.AddOperationalStore(options =>
            {
                options.EndPointUrl = "https://localhost:8081";
                options.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                options.DatabaseName = "DOG";
                options.Collections = new List<Collection>()
                {
                    new Collection()
                    {
                        CollectionName = CollectionName.PersistedGrants,
                        ReserveUnits = 1000

                    }
                };

            });

        }
        protected override void Configure(IServiceProvider provider)
        {
            provider.GetRequiredService<ILoggerFactory>()
               .AddProvider(new XunitTestOutputLoggerProvider(provider.GetRequiredService<ITestOutputHelperAccessor>()));


        }
    }
}
