using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Threading.Tasks;

namespace IdentityServer4.Contrib.Cosmonaut.Interfaces
{
    public interface IFullResourceStore : IResourceStore
    {
        Task StoreAsync(ApiResource apiResource);
        Task RemoveApiResourceAsync(string name);
        Task StoreAsync(IdentityResource apiResource);
        Task RemoveIdentityResourceAsync(string name);

        Task<IdentityResource> FindIdentityResourceAsync(string name);
    }
}
