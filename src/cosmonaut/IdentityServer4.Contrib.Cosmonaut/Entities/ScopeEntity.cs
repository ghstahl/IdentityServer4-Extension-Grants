using Newtonsoft.Json;
using System.Collections.Generic;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class ScopeEntity
    {
        //
        // Summary:
        //     Name of the scope. This is the value a client will use to request the scope.
        [JsonProperty("name")]
        public string Name { get; set; }
        //
        // Summary:
        //     Display name. This value will be used e.g. on the consent screen.
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        //
        // Summary:
        //     Description. This value will be used e.g. on the consent screen.
        [JsonProperty("description")]
        public string Description { get; set; }
        //
        // Summary:
        //     Specifies whether the user can de-select the scope on the consent screen. Defaults
        //     to false.
        [JsonProperty("required")]
        public bool Required { get; set; }
        //
        // Summary:
        //     Specifies whether the consent screen will emphasize this scope. Use this setting
        //     for sensitive or important scopes. Defaults to false.
        [JsonProperty("emphasize")]
        public bool Emphasize { get; set; }
        //
        // Summary:
        //     Specifies whether this scope is shown in the discovery document. Defaults to
        //     true.
        [JsonProperty("showInDiscoveryDocument")]
        public bool ShowInDiscoveryDocument { get; set; }
        //
        // Summary:
        //     List of user-claim types that should be included in the access token.
        [JsonProperty("userClaims")]
        public List<string> UserClaims { get; set; }
    }
}
