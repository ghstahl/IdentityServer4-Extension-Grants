using Newtonsoft.Json;
using System.Collections.Generic;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class ApiResourceEntity : ResourceEntity
    {
        //
        // Summary:
        //     The API secret is used for the introspection endpoint. The API can authenticate
        //     with introspection using the API name and secret.
        [JsonProperty("apiSecrets")]
        public List<SecretEntity> ApiSecrets { get; set; }
        //
        // Summary:
        //     An API must have at least one scope. Each scope can have different settings.
        [JsonProperty("scopes")]
        public List<ScopeEntity> Scopes { get; set; }
    }
}
