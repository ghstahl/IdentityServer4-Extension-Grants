using Xunit.DependencyInjection;

namespace XUnitTest_CosmosDBPersistedGrantStore
{
    internal class DependencyClass : IDependency
    {
        private readonly ITestOutputHelperAccessor _testOutputHelperAccessor;

        public DependencyClass(ITestOutputHelperAccessor testOutputHelperAccessor)
        {
            _testOutputHelperAccessor = testOutputHelperAccessor;
        }
        public int Value => 1;
    }
}
