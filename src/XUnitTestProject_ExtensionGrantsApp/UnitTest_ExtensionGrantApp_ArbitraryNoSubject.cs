using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ArbitraryIdentityExtensionGrant;
using ArbitraryNoSubjectExtensionGrant;
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
        public void ArbitraryNoSubjectExtensionGrantValidator_ValidateAsync_Exception()
        {
            var fakeLogger = A.Fake<ILogger<ArbitraryNoSubjectExtensionGrantValidator>>();
            var arbitraryIdentityExtensionGrantOptions = A.Fake<ArbitraryIdentityExtensionGrantOptions>();
            var d = new ArbitraryNoSubjectExtensionGrantValidator(null,
                null, fakeLogger, null,null);

            d.GrantType.ShouldNotBeNullOrWhiteSpace();
            Should.Throw<Exception>(d.ValidateAsync(null));
        }
        
        [Fact]
        public async Task Mint_arbitrary_no_subject_with_disallowed_claims()
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
                    $"nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'client_namespace': ['no-allowed']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
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
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();

        }
        [Fact]
        public async Task Mint_arbitrary_no_subject_with_malformed_claims()
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
                    $"nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ** malformed **}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
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
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();

        }
        [Fact]
        public async Task Mint_arbitrary_no_subject_with_malformed_amr()
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
                    $"nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['a','b']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAmrs,
                    "[**malformed**]"
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
        public async Task Mint_arbitrary_no_subject_with_malformed_custom_payload()
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
                    $"nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['a','b']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
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
                    "{'I_custom': **malformed**}"
                }
            };
            var result = await client.RequestAsync(paramaters);

            result.Error.ShouldNotBeNullOrEmpty();

        }
        [Fact]
        public async Task Mint_arbitrary_no_subject_with_malformed_audience()
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
                    $"nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'role': ['a','b']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAmrs,
                    "['a','b']"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryAudiences,
                    "[**malformed**]"
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
                    "{'role': ['application']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
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
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();

        }

        [Fact]
        public async Task Mint_arbitrary_no_subject_with_disallowed_scope()
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
                    "{'role': ['application']}"
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
                    "{'role': ['application']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"},
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
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }
    }
}
