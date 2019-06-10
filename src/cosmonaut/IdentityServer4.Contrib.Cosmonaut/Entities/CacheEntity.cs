using Cosmonaut.Attributes;
using Newtonsoft.Json;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class CacheEntity : EntityBase
    {
        [JsonProperty("ttl")]
        public int TTL { get; set; }
        [JsonProperty("key")]
        [CosmosPartitionKey]
        public string Key { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
