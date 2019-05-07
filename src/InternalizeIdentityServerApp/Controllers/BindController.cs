using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModelExtras;
using IdentityModelExtras.Contracts;
using IdentityServer4.ResponseHandling;
using IdentityServer4Extras.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InternalizeIdentityServerApp.Controllers
{
    public class SafeTokenRawResult
    {
        public SafeTokenErrorResult TokenErrorResult { get; set; }
        public TokenResult TokenResult { get; set; }
    }
    public class SafeTokenErrorResult
    {
        public TokenErrorResponse Response { get; set; }
    }
    class CustomPayload
    {
        public string A { get; set; }
        public string B { get; set; }
        public List<string> CList => new List<string>() { "a", "b", "c" };
    }
    [Route("api/auth")]
    [ApiController]
    public class BindController : ControllerBase
    {
        private ILogger<BindController> _logger;
        private ITokenEndpointHandlerExtra _tokenEndpointHandlerExtra;
        private ISelfValidator _selfValidator;

        public BindController(
            ISelfValidator selfValidator,
            ITokenEndpointHandlerExtra tokenEndpointHandlerExtra,
            ILogger<BindController> logger)
        {
            _selfValidator = selfValidator;
            _tokenEndpointHandlerExtra = tokenEndpointHandlerExtra;
            _logger = logger;
        }
        [HttpPost]
        [Route("revocation")]
        public async Task<SafeTokenRawResult> PostRevocationAsync()
        {
            /*
             *TokenTypHint: [refresh_token,subject,access_token]
             */
            var arbResourceOwnerResult = await PostRefreshAsync();
            var revocationRequest = new RevocationRequest()
            {
                Token = arbResourceOwnerResult.TokenResult.Response.RefreshToken,
                ClientId = "arbitrary-resource-owner-client",
                TokenTypHint = "refresh_token",
                RevokeAllSubjects = "true"
            };
            var revocationResult = await _tokenEndpointHandlerExtra.ProcessRawAsync(revocationRequest);
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                RefreshToken = arbResourceOwnerResult.TokenResult.Response.RefreshToken,
                ClientId = "arbitrary-resource-owner-client"
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(refreshTokenRequest);

            var safeResult = new SafeTokenRawResult();
            if (result.TokenErrorResult != null)
            {
                safeResult.TokenErrorResult = new SafeTokenErrorResult()
                {
                    Response = result.TokenErrorResult.Response
                };
            }

            safeResult.TokenResult = result.TokenResult;
            return safeResult;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<SafeTokenRawResult> PostRefreshAsync()
        {
            var arbResourceOwnerResult = await PostArbitraryResourceOwnerAsync();
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                RefreshToken = arbResourceOwnerResult.TokenResult.Response.RefreshToken,
                ClientId = "arbitrary-resource-owner-client"
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(refreshTokenRequest);
            var safeResult = new SafeTokenRawResult();
            if (result.TokenErrorResult != null)
            {
                safeResult.TokenErrorResult = new SafeTokenErrorResult()
                {
                    Response = result.TokenErrorResult.Response
                };
            }

            safeResult.TokenResult = result.TokenResult;
            return safeResult;
        }

        [HttpPost]
        [Route("arbitrary_no_subject")]
        public async Task<SafeTokenRawResult> PostArbitraryNoSubjectAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var extensionGrantRequest = new ArbitraryNoSubjectRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                Scopes = new List<string>()
                {
                    "metal", "nitro", "In", "Flames"
                },
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat", "dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);
            var safeResult = new SafeTokenRawResult();
            if (result.TokenErrorResult != null)
            {
                safeResult.TokenErrorResult = new SafeTokenErrorResult()
                {
                    Response = result.TokenErrorResult.Response
                };
            }

            safeResult.TokenResult = result.TokenResult;
            return safeResult;
        }

        [HttpPost]
        [Route("arbitrary_resource_owner")]
        public async Task<SafeTokenRawResult> PostArbitraryResourceOwnerAsync()
        {
            _logger.LogInformation("Summary Executing...");

            var extensionGrantRequest = new ArbitraryResourceOwnerRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                Scopes = new List<string>()
                {
                    "offline_access", "metal", "nitro", "In", "Flames"
                },
                Subject = "PorkyPig",
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat", "dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);


            var safeResult = new SafeTokenRawResult();
            if (result.TokenErrorResult != null)
            {
                safeResult.TokenErrorResult = new SafeTokenErrorResult()
                {
                    Response = result.TokenErrorResult.Response
                };
            }

            safeResult.TokenResult = result.TokenResult;
            return safeResult;
        }

        [HttpPost]
        [Route("arbitrary_resource_owner_bad_client")]
        public async Task<SafeTokenRawResult> PostArbitraryResourceOwnerBadClientAsync()
        {
            _logger.LogInformation("Summary Executing...");

            var extensionGrantRequest = new ArbitraryResourceOwnerRequest()
            {
                ClientId = Guid.NewGuid().ToString(),
                Scopes = new List<string>()
                {
                    "offline_access", "metal", "nitro", "In", "Flames"
                },
                Subject = "PorkyPig",
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat", "dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);


            var safeResult = new SafeTokenRawResult();
            if (result.TokenErrorResult != null)
            {
                safeResult.TokenErrorResult = new SafeTokenErrorResult()
                {
                    Response = result.TokenErrorResult.Response
                };
            }

            safeResult.TokenResult = result.TokenResult;
            return safeResult;
        }

        [HttpPost]
        [Route("arbitrary_identity")]
        public async Task<SafeTokenRawResult> PostArbitraryIdentityAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var extensionGrantRequest = new ArbitraryIdentityRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                Scopes = new List<string>()
                {
                    "offline_access", "metal", "nitro", "In", "Flames"
                },
                Subject = "PorkyPig",
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat", "dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);
            var a = await _selfValidator.ValidateTokenAsync(result.TokenResult.Response.IdentityToken);
            var safeResult = new SafeTokenRawResult();
            if (result.TokenErrorResult != null)
            {
                safeResult.TokenErrorResult = new SafeTokenErrorResult()
                {
                    Response = result.TokenErrorResult.Response
                };
            }

            safeResult.TokenResult = result.TokenResult;
            return safeResult;
        }
    }
}