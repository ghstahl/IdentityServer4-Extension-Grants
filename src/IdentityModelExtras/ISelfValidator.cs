using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityModelExtras
{
    public interface ISelfValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(string token);
    }
}