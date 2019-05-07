using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using Shouldly;
using Xunit;

namespace XUnitTestProject_ExtensionGrantsApp
{
    public partial class UnitTest_ExtensionGrantApp : IClassFixture<MyTestServerFixture>
    {
        [Fact]
        public async Task Mint_arbitrary_no_subject_with_offline_access()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();

        }

        [Fact]
        public async Task Mint_arbitrary_no_subject_with_offline_access_invalid_claims()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'client_id':['d'],'sub':['dog'],'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Mint_arbitrary_no_subject_with_no_offline_access()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {OidcConstants.TokenRequest.Scope, "nitro metal"},
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{  'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }
    }
}
