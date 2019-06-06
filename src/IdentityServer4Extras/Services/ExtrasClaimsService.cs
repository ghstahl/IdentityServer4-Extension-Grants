using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace IdentityServer4Extras.Services
{
    public class ExtrasClaimsService : IClaimsService
    {
        private DefaultClaimsService _defaultClaimsService;

        public ExtrasClaimsService(
            DefaultClaimsService defaultClaimsService)
        {
            _defaultClaimsService = defaultClaimsService;
        }
        public async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject, Resources resources, bool includeAllIdentityClaims,
            ValidatedRequest request)
        {
            var baseClaims = await _defaultClaimsService.GetIdentityTokenClaimsAsync(subject, resources, includeAllIdentityClaims, request);
            var claims = baseClaims.ToList();
            var client = request.Client as ClientExtra;
            claims.Add(new Claim("client_namespace", client.Namespace));
            return claims;
        }

        public async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Resources resources, ValidatedRequest request)
        {
            var baseClaims = await _defaultClaimsService.GetAccessTokenClaimsAsync(subject, resources, request);
            var claims = baseClaims.ToList();
            var client = request.Client as ClientExtra;
            claims.Add(new Claim("client_namespace", client.Namespace));
            return claims;
        }
    }
}
