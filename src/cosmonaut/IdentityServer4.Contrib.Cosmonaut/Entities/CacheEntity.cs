using Cosmonaut.Attributes;
using Newtonsoft.Json;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class CacheEntity
    {
        [JsonProperty("ttl")]
        public int TTL { get; set; }
        [JsonProperty("id")]

        public string Id { get; set; }

        [JsonProperty("key")]
        [CosmosPartitionKey]
        public string Key { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
