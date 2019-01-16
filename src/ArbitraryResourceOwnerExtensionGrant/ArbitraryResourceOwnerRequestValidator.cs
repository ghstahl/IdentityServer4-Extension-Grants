﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using IdentityServer4Extras.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ArbitraryResourceOwnerExtensionGrant
{
    static class RequestValidationExtensions
    {
        public static bool ValidateFormat<T>(this List<string> errorList, string name, string json)
        {
            bool error = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var values =
                        JsonConvert.DeserializeObject<T>(json);
                }

            }
            catch (Exception)
            {
                error = true;
                errorList.Add($"{name} is malformed!");
            }

            return error;
        }
    }

    public class ArbitraryResourceOwnerRequestValidator
    {
 
        private readonly ILogger<ArbitraryResourceOwnerRequestValidator> _logger;

        private static List<string> _requiredArbitraryArguments;
        private static List<string> RequiredArbitraryArguments => _requiredArbitraryArguments ??
                                                                  (_requiredArbitraryArguments =
                                                                      new List<string>
                                                                      {
                                                                          "client_id"
                                                                      });
        private static List<string> _notAllowedArbitraryClaims;
        private static List<string> NotAllowedArbitraryClaims => _notAllowedArbitraryClaims ??
                                                                 (_notAllowedArbitraryClaims =
                                                                     new List<string>
                                                                     {
                                                                         "client_namespace",
                                                                         ClaimTypes.NameIdentifier,
                                                                         ClaimTypes.AuthenticationMethod,
                                                                         JwtClaimTypes.AccessTokenHash,
                                                                         JwtClaimTypes.Audience,
                                                                         JwtClaimTypes.AuthenticationMethod,
                                                                         JwtClaimTypes.AuthenticationTime,
                                                                         JwtClaimTypes.AuthorizedParty,
                                                                         JwtClaimTypes.AuthorizationCodeHash,
                                                                         JwtClaimTypes.ClientId,
                                                                         JwtClaimTypes.Expiration,
                                                                         JwtClaimTypes.IdentityProvider,
                                                                         JwtClaimTypes.IssuedAt,
                                                                         JwtClaimTypes.Issuer,
                                                                         JwtClaimTypes.JwtId,
                                                                         JwtClaimTypes.Nonce,
                                                                         JwtClaimTypes.NotBefore,
                                                                         JwtClaimTypes.ReferenceTokenId,
                                                                         JwtClaimTypes.SessionId,
                                                                         JwtClaimTypes.Subject,
                                                                         JwtClaimTypes.Scope,
                                                                         JwtClaimTypes.Confirmation,
                                                                         Constants.CustomPayload
                                                                     });

        private static List<string> _oneMustExitsArguments;
        private static List<string> OneMustExitsArguments => _oneMustExitsArguments ??
                                                                  (_oneMustExitsArguments =
                                                                      new List<string>
                                                                      {
                                                                          "subject"
                                                                      });

        public ArbitraryResourceOwnerRequestValidator(
            ILogger<ArbitraryResourceOwnerRequestValidator> logger)
        {
            _logger = logger;
        }
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {

            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[(string)k]);
            var error = false;
            var los = new List<string>();

            var oneMustExistResult = (from item in OneMustExitsArguments
                where rr.Keys.Contains(item)
                select item).ToList();

            if (!oneMustExistResult.Any())
            {
                error = true;
                los.AddRange(OneMustExitsArguments.Select(item => $"[one or the other] {item} is missing!"));
            }
            var result = RequiredArbitraryArguments.Except(rr.Keys);
            if (result.Any())
            {
                error = true;
                los.AddRange(result.Select(item => $"{item} is missing!"));

            }

            // make sure nothing is malformed
            error = los.ValidateFormat<Dictionary<string, List<string>>>(Constants.ArbitraryClaims, raw[Constants.ArbitraryClaims]) || error;
            error = los.ValidateFormat<List<string>>(Constants.ArbitraryAmrs, raw[Constants.ArbitraryAmrs]) || error;
            error = los.ValidateFormat<List<string>>(Constants.ArbitraryAudiences, raw[Constants.ArbitraryAudiences]) || error;

            if (!error)
            {

                var arbitraryClaims = raw[Constants.ArbitraryClaims];
                if (!string.IsNullOrWhiteSpace(arbitraryClaims))
                {
                    var values =
                        JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(arbitraryClaims);
                    var invalidClaims = (from o in values
                        join p in NotAllowedArbitraryClaims on o.Key equals p into t
                        from od in t.DefaultIfEmpty()
                        where od != null
                        select od).ToList();
                    if (invalidClaims.Any())
                    {
                        // not allowed.
                        error = true;
                        foreach (var invalidClaim in invalidClaims)
                        {
                            los.Add($"The arbitrary claim: '{invalidClaim}' is not allowed.");
                        }

                    }
                }
            }
            if (!error)
            {
                var customPayload = raw[Constants.CustomPayload];
                if (!string.IsNullOrWhiteSpace(customPayload))
                {
                    error = !customPayload.IsValidJson();
                    if (error)
                    {
                        los.Add($"{Constants.CustomPayload} is not valid: '{customPayload}'.");
                    }
                }
            }
            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los); ;
            }
        }
    }
}