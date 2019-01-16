using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4Extras.Services
{
    public interface ITokenServiceHookPlugin
    {
        Task<(bool processed, Token token)> OnPostCreateAccessTokenAsync(TokenCreationRequest request, Token token);
        Task<(bool processed, Token token)> OnPostCreateIdentityTokenAsync(TokenCreationRequest request, Token token);
    }
}