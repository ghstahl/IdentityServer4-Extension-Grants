using System;
using System.Collections.Generic;
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

        public ExtrasClaimsService(DefaultClaimsService defaultClaimsService)
        {
            _defaultClaimsService = defaultClaimsService;
        }
        public async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject, Resources resources, bool includeAllIdentityClaims,
            ValidatedRequest request)
        {
            return await _defaultClaimsService.GetIdentityTokenClaimsAsync(subject, resources, includeAllIdentityClaims, request);
        }

        public async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Resources resources, ValidatedRequest request)
        {
            return await _defaultClaimsService.GetAccessTokenClaimsAsync(subject, resources, request);
            
        }
    }
}
