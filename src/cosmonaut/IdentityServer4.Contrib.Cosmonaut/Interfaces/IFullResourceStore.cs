using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Threading.Tasks;

namespace IdentityServer4.Contrib.Cosmonaut.Interfaces
{
    public interface IFullResourceStore : IResourceStore
    {
        Task StoreAsync(ApiResource apiResource);
        Task RemoveApiResourceAsync(string name);
    }
}
