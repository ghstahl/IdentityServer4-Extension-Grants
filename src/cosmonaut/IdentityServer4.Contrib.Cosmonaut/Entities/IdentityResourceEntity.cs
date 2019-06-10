using Newtonsoft.Json;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class IdentityResourceEntity: ResourceEntity
    {
        [JsonProperty("required")]
        public bool Required { get; set; }
        [JsonProperty("emphasize")]
        public bool Emphasize { get; set; }

        [JsonProperty("showInDiscoveryDocument")]
        public bool ShowInDiscoveryDocument { get; set; }
    }
}
