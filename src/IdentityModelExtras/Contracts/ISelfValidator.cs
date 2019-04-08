using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityModelExtras.Contracts
{
    public interface ISelfValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(string token);
    }
}