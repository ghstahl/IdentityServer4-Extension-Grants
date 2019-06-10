using Newtonsoft.Json;
using System;
using System.Text;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class SecretEntity
    {
        //
        // Summary:
        //     Gets or sets the description.
        [JsonProperty("description")]
        public string Description { get; set; }
        //
        // Summary:
        //     Gets or sets the value.
        [JsonProperty("value")]
        public string Value { get; set; }
        //
        // Summary:
        //     Gets or sets the expiration.
        [JsonProperty("expiration")]
        public DateTime? Expiration { get; set; }
        //
        // Summary:
        //     Gets or sets the type of the client secret.
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
