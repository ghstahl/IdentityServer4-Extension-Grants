using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTest_CosmosDBPersistedGrantStore.TestCaseOrdering
{
    public class DisplayNameOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            return testCollections.OrderBy(collection => collection.DisplayName);
        }
    }
}
