using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ArbitraryResourceOwnerExtensionGrant;
using FakeItEasy;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace XUnitTestProject_ExtensionGrantsApp
{
    public partial class UnitTest_ExtensionGrantApp : IClassFixture<MyTestServerFixture>
    {
        public string ClientId => "arbitrary-resource-owner-client";
        public string ClientSecret => "secret";


        [Fact]
        public void ArbitraryResourceOwnerExtensionGrantValidator_ValidateAsync_Exception()
        {
            var fakeLogger = A.Fake<ILogger<ArbitraryResourceOwnerExtensionGrantValidator>>();

            var d = new ArbitraryResourceOwnerExtensionGrantValidator(
                null, null, fakeLogger, null, null, null);

            d.GrantType.ShouldNotBeNullOrWhiteSpace();
            Should.Throw<Exception>(d.ValidateAsync(null));
        }

        [Fact]
        public async Task Mint_arbitrary_resource_owner_with_offline_access()
        {

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"

                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
            result.ExpiresIn.ShouldNotBeNull();
        }



        [Fact]
        public async Task Mint_arbitrary_resource_owner_notallowed_claims()
        {

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'client_namespace': ['no-allowed']}"
                },

                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "2592000"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        public async Task Mint_arbitrary_resource_owner_missing_mallformed_claims()
        {

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'client_namespace': {'no-allowed'}}"
                },

                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "2592000"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
        }
        [Fact]
        public async Task Mint_arbitrary_resource_owner_missing_mallformed_custom_payload()
        {

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'hello': ['allowed']}"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.CustomPayload,
                    "{'I_custom': {'dog':{'allowed']}}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "2592000"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Mint_arbitrary_resource_owner_Lifetime_outofbounds()
        {

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },

                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "2592000"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidRequest);
        }
        [Fact]
        public async Task Mint_arbitrary_resource_owner_missing_subject_and_token()
        {

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
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
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidRequest);
        }
        [Fact]
        public async Task Mint_arbitrary_resource_owner_with_no_offline_access()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"

                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };

            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }
        [Fact]
        public async Task Mint_arbitrary_resource_owner()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAmrs,
                    "['a','b']"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAudiences,
                    "['aud1','aud2']"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.CustomPayload,
                    "{'I_custom': {'dog':['allowed']}}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };

            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

        }

        [Fact]
        public async Task Mint_arbitrary_resource_owner_and_refresh()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };

            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [Fact]
        public async Task Mint_arbitrary_resource_owner_and_refresh_and_revoke()
        {
            /*
                grant_type:arbitrary_resource_owner
                client_id:arbitrary-resource-owner-client
                client_secret:secret
                scope:offline_access nitro metal
                arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
                subject:Ratt
                access_token_lifetime:3600000
             */

            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            var revocationTokenClient = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/revocation",
                ClientId,
                ClientSecret,
                _fixture.MessageHandler);
            paramaters = new Dictionary<string, string>()
            {
                {"token_type_hint", OidcConstants.TokenTypes.RefreshToken},
                {"token", result.RefreshToken}
            };
            await revocationTokenClient.RequestAsync(paramaters);

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidGrant);
        }
        [Fact]
        public async Task Mint_multi_arbitrary_resource_owner_and_refresh_and_revoke()
        {
            var tokenClient = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"

                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await tokenClient.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            // mint a duplicate, this should be 2 refresh tokens.
            var result2 = await tokenClient.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            // first refresh
            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await tokenClient.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result2.RefreshToken}
            };
            result2 = await tokenClient.RequestAsync(paramaters);
            result2.AccessToken.ShouldNotBeNullOrEmpty();
            result2.RefreshToken.ShouldNotBeNull();
            result2.ExpiresIn.ShouldNotBeNull();

            var revocationTokenClient = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/revocation",
                ClientId,
                ClientSecret,
                _fixture.MessageHandler);
            paramaters = new Dictionary<string, string>()
            {
                {IdentityServer4Extras.Constants.RevocationArguments.TokenTypeHint, OidcConstants.TokenTypes.RefreshToken},
                {IdentityServer4Extras.Constants.RevocationArguments.Token, result.RefreshToken},
                {IdentityServer4Extras.Constants.RevocationArguments.RevokeAllSubjects,"true" }
            };
            await revocationTokenClient.RequestAsync(paramaters);

            // try refreshing, these should now fail the refresh.
            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await tokenClient.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidGrant);

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result2.RefreshToken}
            };
            result2 = await tokenClient.RequestAsync(paramaters);
            result2.Error.ShouldNotBeNullOrEmpty();
            result2.Error.ShouldBe(OidcConstants.TokenErrors.InvalidGrant);
        }
    }
}
