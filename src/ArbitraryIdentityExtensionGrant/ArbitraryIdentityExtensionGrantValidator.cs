﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModelExtras;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7Core.Extensions;

namespace ArbitraryIdentityExtensionGrant
{
    public class ArbitraryIdentityExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger _logger;
        private readonly IResourceStore _resourceStore;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ArbitraryIdentityRequestValidator _arbitraryIdentityRequestValidator;
        private PrincipalAugmenter _principalAugmenter;
        private ITokenValidator _tokenValidator;
        private IServiceProvider _serviceProvider;
        private IClientSecretValidator _clientValidator;
        private IHttpContextAccessor _httpContextAccessor;
        private ArbitraryIdentityExtensionGrantOptions _arbitraryIdentityExtensionGrantOptions;


        public ArbitraryIdentityExtensionGrantValidator(
            IServiceProvider serviceProvider,
            IClientSecretValidator clientValidator,
            ITokenValidator tokenValidator,
            IdentityServerOptions options,
            IOptions<ArbitraryIdentityExtensionGrantOptions> arbitraryIdentityExtensionGrantOptions,
            IResourceStore resourceStore,
            ILogger<ArbitraryIdentityExtensionGrantValidator> logger,
            ArbitraryIdentityRequestValidator arbitraryIdentityRequestValidator,
            PrincipalAugmenter principalAugmenter,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _clientValidator = clientValidator;
            _tokenValidator = tokenValidator;
            _logger = logger;
            _options = options;
            _arbitraryIdentityExtensionGrantOptions = arbitraryIdentityExtensionGrantOptions.Value;
            _resourceStore = resourceStore;
            _arbitraryIdentityRequestValidator = arbitraryIdentityRequestValidator;
            _principalAugmenter = principalAugmenter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");

            if (context == null) throw new ArgumentNullException(nameof(context));
            var contextClient = (context.Request.Client as ClientExtra).ShallowCopy();
            context.Request.Client = contextClient;
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
            await _arbitraryIdentityRequestValidator.ValidateAsync(customTokenRequestValidationContext);
            if (customTokenRequestValidationContext.Result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    customTokenRequestValidationContext.Result.Error);
                return;
            }
            // validate HTTP for clients
            if (HttpMethods.IsPost(_httpContextAccessor.HttpContext.Request.Method) && _httpContextAccessor.HttpContext.Request.HasFormContentType)
            {
                // validate client
                var clientResult = await _clientValidator.ValidateAsync(_httpContextAccessor.HttpContext);
                if (!clientResult.IsError)
                {
                    _validatedRequest.SetClient(clientResult.Client);
                }
            }

            /////////////////////////////////////////////
            // check grant type
            /////////////////////////////////////////////
            var grantType = _validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);
         

            _validatedRequest.GrantType = grantType;
            var resource = await _resourceStore.GetAllResourcesAsync();

            var subject = "";
          
            
            if (string.IsNullOrWhiteSpace(subject))
            {
                subject = context.Request.Raw.Get("subject");
            }
            // get user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, subject),
                new Claim("sub", subject),
                new Claim(JwtClaimTypes.IdentityProvider,_arbitraryIdentityExtensionGrantOptions.IdentityProvider)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _principalAugmenter.AugmentPrincipal(principal);
            var userClaimsFinal = new List<Claim>();


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
            // optional stuff;
            var idTokenLifetimeOverride = _validatedRequest.Raw.Get(Constants.IdTokenLifetime);
            if (!string.IsNullOrWhiteSpace(idTokenLifetimeOverride))
            {
                int idTokenLifetime = 0;
                bool error = true;
                if (Int32.TryParse(idTokenLifetimeOverride, out idTokenLifetime))
                {
                    if (idTokenLifetime > 0 && idTokenLifetime <= context.Request.Client.IdentityTokenLifetime)
                    {
                        context.Request.Client.IdentityTokenLifetime = idTokenLifetime;
                        error = false;
                    }
                }

                if (error)
                {
                    var errorDescription =
                        $"{Constants.IdTokenLifetime} out of range.   Must be > 0 and <= configured IdentityTokenLifetime.";
                    LogError(errorDescription);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, errorDescription);
                    return;
                }
            }
          
            context.Result = new GrantValidationResult(
                principal.GetSubjectId(),
                ArbitraryIdentityExtensionGrant.Constants.ArbitraryIdentity,
                userClaimsFinal,
                _arbitraryIdentityExtensionGrantOptions.IdentityProvider);
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
        }
        public string GrantType => Constants.ArbitraryIdentity;
    }
}
