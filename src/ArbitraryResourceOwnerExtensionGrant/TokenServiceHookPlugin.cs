using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4Extras;
using IdentityServer4Extras.Services;
using Newtonsoft.Json;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class TokenServiceHookPlugin : ITokenServiceHookPlugin
    {
        public async Task<(bool processed, Token token)> OnPostCreateAccessTokenAsync(
            TokenCreationRequest request,
            Token token)
        {
            var grantType = request.ValidatedRequest.Raw[OidcConstants.TokenRequest.GrantType];
            if (grantType != Constants.ArbitraryResourceOwner)
            {
                return (false, null);
            }

            var arbitraryClaims = request.ValidatedRequest.Raw[Constants.ArbitraryClaims];
            if (!string.IsNullOrWhiteSpace(arbitraryClaims))
            {
                var values =
                    JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(arbitraryClaims);
                var finalClaims =
                    from item in values
                    from c in item.Value
                    select new Claim(item.Key, c);
                foreach (var claim in finalClaims)
                {
                    token.Claims.Add(claim);
                }
            }

            var amr = request.ValidatedRequest.Raw[Constants.ArbitraryAmrs];
            if (!string.IsNullOrWhiteSpace(amr))
            {
                var values =
                    JsonConvert.DeserializeObject<List<string>>(amr);
                var finalClaims = from item in values
                    select new Claim(JwtClaimTypes.AuthenticationMethod, item);

                foreach (var claim in finalClaims)
                {
                    token.Claims.Add(claim);
                }
            }

            var audiences = request.ValidatedRequest.Raw[Constants.ArbitraryAudiences];
            if (!string.IsNullOrWhiteSpace(audiences))
            {
                var values =
                    JsonConvert.DeserializeObject<List<string>>(audiences);
                var finalClaims = from item in values
                    select new Claim(JwtClaimTypes.Audience, item);

                foreach (var claim in finalClaims)
                {
                    token.Claims.Add(claim);
                }
            }

            var customPayload = request.ValidatedRequest.Raw[Constants.CustomPayload];
            if (!string.IsNullOrWhiteSpace(customPayload))
            {
                token.Claims.Add(new Claim(Constants.CustomPayload, customPayload,
                    IdentityServerConstants.ClaimValueTypes.Json));

            }

            var clientExtra = request.ValidatedRequest.Client as ClientExtra;
            if (!string.IsNullOrEmpty(clientExtra.Namespace))
            {
                token.Claims.Add(new Claim("client_namespace", clientExtra.Namespace));

            }

            return (true, token);
        }

        public async Task<(bool processed, Token token)> OnPostCreateIdentityTokenAsync(TokenCreationRequest request, Token token)
        {
            return (false, null);
        }
    }
}