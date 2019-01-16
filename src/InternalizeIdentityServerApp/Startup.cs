using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArbitraryIdentityExtensionGrant.Extensions;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using IdentityModelExtras.Extensions;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7Core;
using P7Core.IRules;
using P7Core.ObjectContainers.Extensions;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;

namespace InternalizeIdentityServerApp
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _hostingEnvironment = env;
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddObjectContainer();  // use this vs a static to cache class data.
            services.AddOptions();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    corsBuilder => corsBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            var clients = Configuration.LoadClientsFromSettings();
            var apiResources = Configuration.LoadApiResourcesFromSettings();
            var identityResources = Configuration.LoadIdentityResourcesFromSettings();

            bool useRedis = Convert.ToBoolean(Configuration["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVault"]);
            bool useKeyVaultSigning = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVaultSigning"]);

            var builder = services
               .AddIdentityServer(options => { options.InputLengthRestrictions.RefreshToken = 256; })
               .AddInMemoryIdentityResources(identityResources)
               .AddInMemoryApiResources(apiResources)
               .AddInMemoryClientsExtra(clients)
               .AddIdentityServer4Extras()
               .AddProfileServiceManager()
               .AddArbitraryOwnerResourceExtensionGrant()
               .AddArbitraryIdentityExtensionGrant()
               .AddArbitraryNoSubjectExtensionGrant();
            // My Replacement Services.
            if (useRedis)
            {
                var redisConnectionString = Configuration["appOptions:redis:redisConnectionString"];
                builder.AddOperationalStore(options =>
                {
                    options.RedisConnectionString = redisConnectionString;
                    options.Db = 1;
                })
                    .AddRedisCaching(options =>
                    {
                        options.RedisConnectionString = redisConnectionString;
                        options.KeyPrefix = "prefix";
                    });

                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });
            }
            else
            {
                builder.AddInMemoryPersistedGrants();
                services.AddDistributedMemoryCache();
            }
            if (useKeyVault)
            {
                builder.AddKeyVaultCredentialStore();
                services.AddKeyVaultTokenCreateServiceTypes();
                services.AddKeyVaultTokenCreateServiceConfiguration(Configuration);
                if (useKeyVaultSigning)
                {
                    // this signs the token using azure keyvault to do the actual signing
                    builder.AddKeyVaultTokenCreateService();
                }
            }
            else
            {
                builder.AddDeveloperSigningCredential();
            }

            // my replacement services.
            builder.AddRefreshTokenRevokationGeneratorWorkAround();

            builder.AddPluginHostClientSecretValidator();
            builder.AddNoSecretRefreshClientSecretValidator();

            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra
            builder.SwapOutTokenResponseGenerator();
            builder.SwapOutDefaultTokenService();
            builder.SwapOutScopeValidator();
            builder.SwapOutTokenRevocationRequestValidator();

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryIdentityExtentionGrantTypes();
            services.AddIdentityModelExtrasTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            builder.AddProtectedRefreshTokenKeyObfuscator();
            // Request Tracker
            services.AddIdentityServerRequestTrackerMiddleware();

            // my configurations
            services.Configure<Options.RedisAppOptions>(Configuration.GetSection("appOptions:redis"));
            services.Configure<Options.KeyVaultAppOptions>(Configuration.GetSection("appOptions:keyVault"));
            services.RegisterP7CoreConfigurationServices(Configuration);


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["self:authority"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudiences = new List<string>()
                        {
                            $"{options.Authority}/Resources"
                        }
                    };
                });
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Build the intermediate service provider then return it
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRewriter(new RewriteOptions().Add(new RewriteLowerCaseRule()));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseIdentityServerRequestTrackerMiddleware();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
