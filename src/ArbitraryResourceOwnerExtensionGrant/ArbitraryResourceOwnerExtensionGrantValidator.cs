using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class ArbitraryResourceOwnerExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryResourceOwnerExtensionGrantValidator> _logger;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ArbitraryResourceOwnerRequestValidator _arbitraryResourceOwnerRequestValidator;
        private PrincipalAugmenter _principalAugmenter;

        private IClientSecretValidator _clientValidator;
        private IHttpContextAccessor _httpContextAccessor;


        public ArbitraryResourceOwnerExtensionGrantValidator(
            IdentityServerOptions options,
            IClientSecretValidator clientValidator,
            ILogger<ArbitraryResourceOwnerExtensionGrantValidator> logger,
            ArbitraryResourceOwnerRequestValidator arbitraryResourceOwnerRequestValidator,
            PrincipalAugmenter principalAugmenter,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _options = options;
            _clientValidator = clientValidator;
            _arbitraryResourceOwnerRequestValidator = arbitraryResourceOwnerRequestValidator;
            _principalAugmenter = principalAugmenter;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");


            if (context == null) throw new ArgumentNullException(nameof(context));
            var raw = context.Request.Raw;
            _validatedRequest = new ValidatedTokenRequest
            {
                Raw = raw ?? throw new ArgumentNullException(nameof(raw)),
                Options = _options
            };
            var customTokenRequestValidationContext = new CustomTokenRequestValidationContext()
            {
                Result = new TokenRequestValidationResult(_validatedRequest)
            };
            await _arbitraryResourceOwnerRequestValidator.ValidateAsync(customTokenRequestValidationContext);
            if (customTokenRequestValidationContext.Result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    customTokenRequestValidationContext.Result.Error);
                return;
            }


            var clientResult = await _clientValidator.ValidateAsync(_httpContextAccessor.HttpContext);
            if (!clientResult.IsError)
            {
                _validatedRequest.SetClient(clientResult.Client);
            }

            /////////////////////////////////////////////
            // get the grant type
            // NOTE: The identityserver4 pipeline before us validated to obvious stuff
            /////////////////////////////////////////////
            var grantType = _validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);

            _validatedRequest.GrantType = grantType;

            var subject = "";
            if (string.IsNullOrWhiteSpace(subject))
            {
                subject = context.Request.Raw.Get("subject");
            }

            // get user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, subject),
                new Claim("sub", subject)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _principalAugmenter.AugmentPrincipal(principal);

            // optional stuff;
            var accessTokenLifetimeOverride = _validatedRequest.Raw.Get(Constants.AccessTokenLifetime);
            if (!string.IsNullOrWhiteSpace(accessTokenLifetimeOverride))
            {
                int accessTokenLifetime = 0;
                bool error = true;
                if (Int32.TryParse(accessTokenLifetimeOverride, out accessTokenLifetime))
                {
                    if (accessTokenLifetime > 0 && accessTokenLifetime <= context.Request.AccessTokenLifetime)
                    {
                        context.Request.AccessTokenLifetime = accessTokenLifetime;
                        error = false;
                    }
                }
                if (error)
                {
                    var errorDescription =
                        $"{Constants.AccessTokenLifetime} out of range.   Must be > 0 and <= configured AccessTokenLifetime.";
                    LogError(errorDescription);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, errorDescription);
                    return;
                }
            }

            context.Result = new GrantValidationResult(principal.GetSubjectId(),
                ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner);
        }

        [ExcludeFromCodeCoverage]
        private void LogError(string message = null, params object[] values)
        {
            if (message.IsPresent())
            {
                try
                {
                    _logger.LogError(message, values);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error logging {exception}", ex.Message);
                }
            }

            //  var details = new global::IdentityServer4.Logging.TokenRequestValidationLog(_validatedRequest);
            //  _logger.LogError("{details}", details);
        }

        public string GrantType => Constants.ArbitraryResourceOwner;
    }
}
