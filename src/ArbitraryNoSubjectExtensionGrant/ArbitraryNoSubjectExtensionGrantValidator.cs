using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArbitraryNoSubjectExtensionGrant
{
    public class ArbitraryNoSubjectExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryNoSubjectExtensionGrantValidator> _logger;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ArbitraryNoSubjectRequestValidator _arbitraryNoSubjectRequestValidator;
        private PrincipalAugmenter _principalAugmenter;
        private IServiceProvider _serviceProvider;
        private IClientSecretValidator _clientValidator;
        private IHttpContextAccessor _httpContextAccessor;

        public ArbitraryNoSubjectExtensionGrantValidator(
            IServiceProvider serviceProvider,
            IdentityServerOptions options,
            IClientSecretValidator clientValidator,
            ILogger<ArbitraryNoSubjectExtensionGrantValidator> logger,
            ArbitraryNoSubjectRequestValidator arbitraryNoSubjectRequestValidator,
            PrincipalAugmenter principalAugmenter,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _clientValidator = clientValidator;
            _options = options;
            _arbitraryNoSubjectRequestValidator = arbitraryNoSubjectRequestValidator;
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
            await _arbitraryNoSubjectRequestValidator.ValidateAsync(customTokenRequestValidationContext);
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
            // check grant type
            /////////////////////////////////////////////
            var grantType = _validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType.IsMissing())
            {
                LogError("Grant type is missing");
                context.Result = new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
                return;
            }
            if (grantType.Length > _options.InputLengthRestrictions.GrantType)
            {
                LogError("Grant type is too long");
                context.Result = new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
                return;
            }

            _validatedRequest.GrantType = grantType;
            context.Result = new GrantValidationResult();
        }
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
        public string GrantType => Constants.ArbitraryNoSubject;
    }
}
