﻿using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault.Models;

namespace P7IdentityServer4
{
    public interface ITokenSigningCredentialStore
    {
        Task<X509Certificate2> GeX509Certificate2Async();
    }
}