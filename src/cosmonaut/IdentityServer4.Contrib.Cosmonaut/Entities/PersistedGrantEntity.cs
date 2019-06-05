using System;
using System.Collections.Generic;
using System.Text;
using Cosmonaut.Attributes;
using Newtonsoft.Json;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class PersistedGrantEntity
    {
        [JsonProperty("id")]

        public string Id { get; set; }

        [JsonProperty("ttl")]
        public int TTL { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }

        [JsonProperty("key")]
        [CosmosPartitionKey]
        public string Key { get; set; }

        //
        // Summary:
        //     Gets the type.
        public string Type { get; set; }
        //
        // Summary:
        //     Gets the subject identifier.
        public string SubjectId { get; set; }
        //
        // Summary:
        //     Gets the client identifier.
        public string ClientId { get; set; }
        //
        // Summary:
        //     Gets or sets the creation time.
        public DateTime CreationTime { get; set; }
        //
        // Summary:
        //     Gets or sets the expiration.
        public DateTime? Expiration { get; set; }
        //
        // Summary:
        //     Gets or sets the data.
        public string Data { get; set; }
    }
}
