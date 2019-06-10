using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Contrib.Cosmonaut.Entities
{
    public class EntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
