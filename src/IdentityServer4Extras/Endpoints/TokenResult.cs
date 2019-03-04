using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4Extras.Endpoints
{
    public class TokenResponseExtra
    {
        public TokenResponseExtra() { }

        public string IdentityToken { get; set; }
        public string AccessToken { get; set; }
        public int AccessTokenLifetime { get; set; }
        public string RefreshToken { get; set; }
        public Dictionary<string, object> Custom { get; set; }
    }

    internal static class TokenResponseExtraExtensions
    {
        public static TokenResponseExtra ToTokenResponseExtra(this TokenResponse tokenResponse)
        {
            return new TokenResponseExtra()
            {
                AccessToken = tokenResponse.AccessToken,
                AccessTokenLifetime = tokenResponse.AccessTokenLifetime,
                Custom = tokenResponse.Custom,
                IdentityToken = tokenResponse.IdentityToken,
                RefreshToken = tokenResponse.RefreshToken
            };


        }
    }
    public class TokenResult : IEndpointResult
    {
        public TokenResponseExtra Response { get; set; }

        public TokenResult(TokenResponseExtra response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            Response = response;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                id_token = Response.IdentityToken,
                access_token = Response.AccessToken,
                refresh_token = Response.RefreshToken,
                expires_in = Response.AccessTokenLifetime,
                token_type = OidcConstants.TokenResponse.BearerTokenType
            };

            if (Response.Custom.IsNullOrEmpty())
            {
                await context.Response.WriteJsonAsync(dto);
            }
            else
            {
                var jobject = ObjectSerializer.ToJObject(dto);
                jobject.AddDictionary(Response.Custom);

                await context.Response.WriteJsonAsync(jobject);
            }
        }

        internal class ResultDto
        {
            public string id_token { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
        }
 
        
    }
}