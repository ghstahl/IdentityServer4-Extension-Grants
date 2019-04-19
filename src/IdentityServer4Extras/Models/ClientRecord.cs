using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace IdentityServer4Extras.Models
{
    public partial class ClientRecord
    {
        [JsonIgnore]
        public string ClientId { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        List<string> _secrets;
        [JsonProperty("secrets")]
        public List<string> Secrets
        {
            get
            {
                if (_secrets == null)
                {
                    _secrets = new List<string>();
                }
                return _secrets;
            }
            set
            {
                _secrets = value;
            }
        }
        List<string> _allowedScopes;
        [JsonProperty("allowedScopes")]
        public List<string> AllowedScopes
        {
            get
            {
                if (_allowedScopes == null)
                {
                    _allowedScopes = new List<string>();
                }
                return _allowedScopes;
            }
            set
            {
                _allowedScopes = value;
            }
        }
        List<string> _allowedGrantTypes;
        [JsonProperty("AllowedGrantTypes")]
        public List<string> AllowedGrantTypes
        {
            get
            {
                if (_allowedGrantTypes == null)
                {
                    _allowedGrantTypes = new List<string>();
                }
                return _allowedGrantTypes;
            }
            set
            {
                _allowedGrantTypes = value;
            }
        }

        [JsonProperty("IdentityTokenLifetime")]
        public int IdentityTokenLifetime { get; set; }

        [JsonProperty("AccessTokenLifetime")]
        public int AccessTokenLifetime { get; set; }

        [JsonProperty("AuthorizationCodeLifetime")]
        public int AuthorizationCodeLifetime { get; set; }

        [JsonProperty("AbsoluteRefreshTokenLifetime")]
        public int AbsoluteRefreshTokenLifetime { get; set; }

        [JsonProperty("FrontChannelLogoutSessionRequired")]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [JsonProperty("FrontChannelLogoutUri")]
        public string FrontChannelLogoutUri { get; set; }

        [JsonProperty("SlidingRefreshTokenLifetime")]
        public int SlidingRefreshTokenLifetime { get; set; }
        List<string> _postLogoutRedirectUris;
        [JsonProperty("PostLogoutRedirectUris")]
        public List<string> PostLogoutRedirectUris
        {
            get
            {
                if (_postLogoutRedirectUris == null)
                {
                    _postLogoutRedirectUris = new List<string>();
                }
                return _postLogoutRedirectUris;
            }
            set
            {
                _postLogoutRedirectUris = value;
            }
        }

        List<string> _redirectUris;
        [JsonProperty("RedirectUris")]
        public List<string> RedirectUris
        {
            get
            {
                if (_redirectUris == null)
                {
                    _redirectUris = new List<string>();
                }
                return _redirectUris;
            }
            set
            {
                _redirectUris = value;
            }
        }

        [JsonProperty("RefreshTokenUsage")]
        public long RefreshTokenUsage { get; set; }

        [JsonProperty("AccessTokenType")]
        public long AccessTokenType { get; set; }

        [JsonProperty("AllowOfflineAccess")]
        public bool AllowOfflineAccess { get; set; }

        [JsonProperty("RequireClientSecret")]
        public bool RequireClientSecret { get; set; }

        [JsonProperty("RequireConsent")]
        public bool RequireConsent { get; set; }

        [JsonProperty("RequireRefreshClientSecret")]
        public bool RequireRefreshClientSecret { get; set; }

        [JsonProperty("ClientClaimsPrefix")]
        public string ClientClaimsPrefix { get; set; }

        [JsonProperty("Namespace")]
        public string Namespace { get; set; }


    }
}
