using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    class TimeStamp
    {
        public string UtcTime { get; set; }
    }
    public class KeyVaultCache : IKeyVaultCache
    {
        private const string CacheValidationKey = "62777f80-2be6-4882-b1cc-28a202d423e6";
        private const string CacheValidationKeyTimeStamp = "104895ef-71cc-4c98-9a72-d3ffea75977b";
        private readonly AzureKeyVaultTokenSigningServiceOptions _keyVaultOptions;
        private readonly AzureKeyVaultAuthentication _azureKeyVaultAuthentication;
        private List<KeyBundle> _keyBundles;
        private IMemoryCache _cache;
        private ILogger _logger;
        private readonly DefaultCache<CacheData> _cachedData;
        private DefaultCache<TimeStamp> _cacheTimeStamp;

        public KeyVaultCache(
            IOptions<AzureKeyVaultTokenSigningServiceOptions> keyVaultOptions,
            IMemoryCache cache,
            ILogger<KeyVaultCache> logger)
        {
            _keyVaultOptions = keyVaultOptions.Value;
            _azureKeyVaultAuthentication = new AzureKeyVaultAuthentication(_keyVaultOptions.ClientId, _keyVaultOptions.ClientSecret);
            _cache = cache;
            _logger = logger;
            _cachedData = new DefaultCache<CacheData>(_cache);
            _cacheTimeStamp = new DefaultCache<TimeStamp>(_cache);
        }

        public async Task<DateTime?> GetKeyVaultCacheDataUtcAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var timeStamp = await _cacheTimeStamp.GetAsync(CacheValidationKeyTimeStamp);
            if (timeStamp == null)
                return null;
            DateTime nowUtc = DateTime.ParseExact(timeStamp.UtcTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return nowUtc;
        }

        public async Task<CacheData> GetKeyVaultCacheDataAsync(CancellationToken cancellationToken)
        {
            var cachedData = await _cachedData.GetAsync(CacheValidationKey);
            if (cachedData == null)
            {
                await RefreshCacheData();
                cachedData = await _cachedData.GetAsync(CacheValidationKey);
                // TODO: need to look into this more. We can't do our scheduler scheme for this.
                // Probably should be an entry in REDIS with a timeout for refresh.
            }
            return cachedData;
        }

        public async Task RefreshCacheFromSourceAsync(CancellationToken cancellationToken)
        {
            await RefreshCacheData();
        }
        string StipPort(string url)
        {
            UriBuilder u1 = new UriBuilder(url);
            u1.Port = -1;
            string clean = u1.Uri.ToString();

            return clean;
        }
        private async Task RefreshCacheData()
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime cacheTime = DateTime.UtcNow.AddHours(-7);

                var timeStamp = await _cacheTimeStamp.GetAsync(CacheValidationKeyTimeStamp);
                if (timeStamp != null)
                {
                    cacheTime = DateTime.ParseExact(timeStamp.UtcTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }

                if (now < cacheTime.AddHours(6))
                    return;

                _keyBundles = await GetKeyBundleVersionsAsync();
                var queryKbs = from item in _keyBundles
                               where item.Attributes.Enabled != null && (bool)item.Attributes.Enabled
                               && (item.Attributes.Expires == null || item.Attributes.Expires > DateTime.UtcNow)
                               select item;
                _keyBundles = queryKbs.ToList();

                var latestKB = GetLatestKeyBundleWithRolloverDelay(_keyBundles);

                X509Certificate2 x509Certificate2 = null;
                if (!_keyVaultOptions.UseKeyVaultSigning)
                {
                    var x509Certificate2s = await GetAllCertificateVersions();
                    x509Certificate2 = GetLatestCertificateWithRolloverDelay(x509Certificate2s);
                }

                var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);
                var queryRsaSecurityKeys = from item in _keyBundles
                                           let c = new RsaSecurityKey(keyVaultClient.ToRSA(item))
                                           {
                                               KeyId = StipPort(item.KeyIdentifier.Identifier)
                                           }
                                           select c;

                var jwks = new List<JsonWebKey>();
                foreach (var keyBundle in _keyBundles)
                {
                    jwks.Add(new JsonWebKey(keyBundle.Key.ToString()));
                }

                var jwk = latestKB.Key;
                var kid = latestKB.KeyIdentifier;               

                var parameters = new RSAParameters
                {
                    Exponent =  jwk.E,
                    Modulus = jwk.N
                };
                var securityKey = new RsaSecurityKey(parameters)
                {
                    KeyId = jwk.Kid,
                };

                SigningCredentials signingCredentials;
                if (_keyVaultOptions.UseKeyVaultSigning)
                {
                    signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
                }
                else
                {
                    signingCredentials = new MySigningCredentials(x509Certificate2);
                }
            
               
                CacheData cacheData = new CacheData()
                {
                    RsaSecurityKeys = queryRsaSecurityKeys.ToList(),
                    SigningCredentials = signingCredentials,
                    JsonWebKeys = jwks,
                    KeyIdentifier = kid,
                    X509Certificate2 = x509Certificate2
                };
                await _cachedData.SetAsync(CacheValidationKey, cacheData, TimeSpan.FromHours(6));

                await _cacheTimeStamp.SetAsync(CacheValidationKeyTimeStamp, new TimeStamp()
                {
                    UtcTime = DateTime.UtcNow.ToString("O")
                }, TimeSpan.FromHours(6));
                
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"KeyVault RefreshCacheData fatal exception");
                throw;
            }

        }

        private async Task<List<X509Certificate2>> GetAllCertificateVersions()
        {
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);

            var certificates = new List<X509Certificate2>();

            // Get the first page of certificates
            var certificateItemsPage = await keyVaultClient.GetCertificateVersionsAsync(_keyVaultOptions.KeyVaultUrl, _keyVaultOptions.KeyIdentifier);
            while (true)
            {
                foreach (var certificateItem in certificateItemsPage)
                {
                    // Ignored disabled or expired certificates
                    if (certificateItem.Attributes.Enabled == true &&
                        (certificateItem.Attributes.Expires == null || certificateItem.Attributes.Expires > DateTime.UtcNow))
                    {
                        var certificateVersionBundle = await keyVaultClient.GetCertificateAsync(certificateItem.Identifier.Identifier);
                        var certificatePrivateKeySecretBundle = await keyVaultClient.GetSecretAsync(certificateVersionBundle.SecretIdentifier.Identifier);
                        var privateKeyBytes = Convert.FromBase64String(certificatePrivateKeySecretBundle.Value);
                        var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes, (string)null, X509KeyStorageFlags.MachineKeySet);

                        certificates.Add(certificateWithPrivateKey);
                    }
                }

                if (certificateItemsPage.NextPageLink == null)
                {
                    break;
                }
                else
                {
                    // Get the next page
                    certificateItemsPage = await keyVaultClient.GetCertificateVersionsNextAsync(certificateItemsPage.NextPageLink);
                }
            }

            return certificates;
        }
        private async Task<List<KeyBundle>> GetKeyBundleVersionsAsync()
        {
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);

            List<KeyItem> keyItems = new List<KeyItem>();

            var page = await keyVaultClient.GetKeyVersionsAsync(_keyVaultOptions.KeyVaultUrl, _keyVaultOptions.KeyIdentifier);
            keyItems.AddRange(page);
            while (!string.IsNullOrWhiteSpace(page.NextPageLink))
            {
                page = await keyVaultClient.GetKeyVersionsNextAsync(page.NextPageLink);
                keyItems.AddRange(page);

            }
            var keyBundles = new List<KeyBundle>();

            foreach (var keyItem in keyItems)
            {
                var keyBundle = await keyVaultClient.GetKeyAsync(keyItem.Identifier.Identifier);
                keyBundles.Add(keyBundle);
            }

            return keyBundles;
        }
        private X509Certificate2 GetLatestCertificateWithRolloverDelay(List<X509Certificate2> certificates)
        {
            // First limit the search to just those certificates that have existed longer than the rollover delay.
            var rolloverCutoff = DateTime.UtcNow.AddHours(-_keyVaultOptions.RolloverDelayHours);
            var potentialCerts = certificates.Where(c => c.NotBefore < rolloverCutoff);

            // If no certs could be found, then widen the search to any usable certificate.
            if (!potentialCerts.Any())
            {
                potentialCerts = certificates.Where(c => c.NotBefore < DateTime.UtcNow);
            }

            // Of the potential certs, return the newest one.
            return potentialCerts
                .OrderByDescending(c => c.NotBefore)
                .FirstOrDefault();
        }

        private KeyBundle GetLatestKeyBundleWithRolloverDelay(List<KeyBundle> kbs)
        {
            // First limit the search to just those certificates that have existed longer than the rollover delay.
            var rolloverCutoff = DateTime.UtcNow.AddHours(-_keyVaultOptions.RolloverDelayHours);
            var potentialCerts = kbs.Where(c => c.Attributes.NotBefore < rolloverCutoff);

            // If no certs could be found, then widen the search to any usable certificate.
            if (!potentialCerts.Any())
            {
                potentialCerts = kbs.Where(c => c.Attributes.NotBefore < DateTime.UtcNow);
            }

            // Of the potential certs, return the newest one.
            return potentialCerts
                .OrderByDescending(c => c.Attributes.NotBefore)
                .FirstOrDefault();
        }

         

        private async Task<X509Certificate2> GetX509Certificate2Async()
        {
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);
          
            var certificateSecret = await keyVaultClient.GetSecretAsync(_keyVaultOptions.KeyVaultUrl,_keyVaultOptions.KeyIdentifier);
            var certificate = System.Convert.FromBase64String(certificateSecret.Value);
           
            // NOTE: creating this cert from an azure keyvault throws exceptions if you don't pass in those storage flags.
            var x509Certificate2 = new X509Certificate2(certificate, (string) null, X509KeyStorageFlags.MachineKeySet |
                                                                           X509KeyStorageFlags.PersistKeySet |
                                                                           X509KeyStorageFlags.Exportable);
            return x509Certificate2;
        }
    }
}