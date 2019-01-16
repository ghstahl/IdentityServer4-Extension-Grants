﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    public class CacheData
    {
        public List<RsaSecurityKey> RsaSecurityKeys { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public List<JsonWebKey> JsonWebKeys { get; set; }
        public KeyIdentifier KeyIdentifier { get; set; }
        public X509Certificate2 X509Certificate2 { get; set; }
    }
}