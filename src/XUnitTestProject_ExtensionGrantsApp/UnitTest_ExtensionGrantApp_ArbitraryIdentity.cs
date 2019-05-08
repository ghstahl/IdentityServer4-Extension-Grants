using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ArbitraryIdentityExtensionGrant;
using ArbitraryResourceOwnerExtensionGrant;
using FakeItEasy;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace XUnitTestProject_ExtensionGrantsApp
{
    public partial class UnitTest_ExtensionGrantApp : IClassFixture<MyTestServerFixture>
    {
        [Fact]
        public void ArbitraryIdentityExtensionGrantValidator_ValidateAsync_Exception()
        {
            var fakeLogger = A.Fake<ILogger<ArbitraryIdentityExtensionGrantValidator>>();
            var fakeOptions = A.Fake<IOptions<ArbitraryIdentityExtensionGrantOptions>>();
            var arbitraryIdentityExtensionGrantOptions = A.Fake<ArbitraryIdentityExtensionGrantOptions>();
            A.CallTo(() => fakeOptions.Value).Returns(arbitraryIdentityExtensionGrantOptions);

            var d = new ArbitraryIdentityExtensionGrantValidator(null,
                null, null, null, fakeOptions, 
                null,
                fakeLogger, null, null, null);
            
            d.GrantType.ShouldNotBeNullOrWhiteSpace();
            Should.Throw<Exception>(d.ValidateAsync(null));
        }
        [Fact]
        public async Task Mint_arbitrary_identity_missing_subject()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
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
        public async Task Mint_arbitrary_identity_with_malformed_custom_payload()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'hello': ['world']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "4400"},
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.CustomPayload,
                    "{'I_custom': ['malformed':['allowed']}}"
                }
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
        }
        [Fact]
        public async Task Mint_arbitrary_identity_with_malformed_claims()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'hello': {'malformed']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "4400"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
        }
        [Fact]
        public async Task Mint_arbitrary_identity_with_disallowed_claims()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'client_namespace': ['no-allowed']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "4400"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
        }
        [Fact]
        public async Task Mint_arbitrary_identity_malformed_amr()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "4400"},
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAmrs,
                    "[malformed]"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAudiences,
                    "['aud1','aud2']"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.CustomPayload,
                    "{'I_custom': {'dog':['allowed']}}"
                }
            };
            var result = await client.RequestAsync(paramaters);
       
            result.Error.ShouldNotBeNullOrEmpty();

        }
        [Fact]
        public async Task Mint_arbitrary_identity_malformed_aud()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "4400"},

                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAudiences,
                    "[malformed]"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.CustomPayload,
                    "{'I_custom': {'dog':['allowed']}}"
                }
            };
            var result = await client.RequestAsync(paramaters);

            result.Error.ShouldNotBeNullOrEmpty();

        }
        [Fact]
        public async Task Mint_arbitrary_identity_with_offline_access()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "4400"},
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAmrs,
                    "['a','b']"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAudiences,
                    "['aud1','aud2']"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.CustomPayload,
                    "{'I_custom': {'dog':['allowed']}}"
                }
            };
            var result = await client.RequestAsync(paramaters);
            result.ErrorDescription.ShouldBeNull();
            result.Error.ShouldBeNull();

            result.IdentityToken.ShouldNotBeNullOrEmpty();
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Mint_arbitrary_identity_with_NO_offline_access()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.ErrorDescription.ShouldBeNull();
            result.Error.ShouldBeNull();

            result.IdentityToken.ShouldNotBeNullOrEmpty();
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
        }
        [Fact]
        public async Task Mint_arbitrary_identity_with_outofbounds_AccessTokenLifetime()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "320000000"}
            };
            var result = await client.RequestAsync(paramaters);
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();


        }
        [Fact]
        public async Task Mint_arbitrary_identity_with_outofbounds_IdTokenLifetime()
        {
            var client = new TokenClient(
                _fixture.TestServer.BaseAddress + "connect/token",
                ClientId,
                _fixture.MessageHandler);

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryIdentity},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"nitro metal"
                },
                {
                    ArbitraryIdentityExtensionGrant.Constants.Subject, "Ratt"
                },
                {
                    ArbitraryIdentityExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryIdentityExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {ArbitraryIdentityExtensionGrant.Constants.IdTokenLifetime, "3200000001"}
            };
            var result = await client.RequestAsync(paramaters);
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();


        }
    }
}
