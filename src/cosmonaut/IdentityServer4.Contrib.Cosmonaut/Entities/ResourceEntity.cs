using Cosmonaut.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class ResourceEntity : EntityBase
    {
        //
        // Summary:
        //     Indicates if this resource is enabled. Defaults to true.
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        //
        // Summary:
        //     The unique name of the resource.
        [CosmosPartitionKey]
        [JsonProperty("name")]
        public string Name { get; set; }
        //
        // Summary:
        //     Display name of the resource.
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        //
        // Summary:
        //     Description of the resource.
        [JsonProperty("description")]
        public string Description { get; set; }
        //
        // Summary:
        //     List of accociated user claims that should be included when this resource is
        //     requested.
        [JsonProperty("userClaims")]
        public List<string> UserClaims { get; set; }
        //
        // Summary:
        //     Gets or sets the custom properties for the resource.
        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; }
    }
}
