using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ArbitraryIdentityExtensionGrant;
using IdentityServer4.Configuration;
using IdentityServer4ExtensionGrants.Rollup.Extensions;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Extensions;
using IdentityServerRequestTracker.Services;
using InternalizeIdentityServerApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using P7IdentityServer4.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace InternalizeIdentityServerApp
{
    public class Startup : IExtensionGrantsRollupRegistrations
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
            services.AddTransient<IIdentityServerRequestTrackerEvaluator, MyIdentityServerRequestTrackerEvaluator>();
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
            services.AddExtensionGrantsRollup(this);


            services.Configure<ArbitraryIdentityExtensionGrantOptions>(options => { options.IdentityProvider = "Demo"; });
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "ToDo API"

                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            return services.BuildServiceProvider();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui(HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


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

            app.ValidateIdentityServer();
        }

        public void AddIdentityResources(IServiceCollection services, IIdentityServerBuilder builder)
        {
            var identityResources = Configuration.LoadIdentityResourcesFromSettings();
            builder.AddInMemoryIdentityResources(identityResources);
        }

        public void AddClients(IServiceCollection services, IIdentityServerBuilder builder)
        {
            var clients = Configuration.LoadClientsFromSettings();
            builder.AddInMemoryClientsExtra(clients);
        }

        public void AddApiResources(IServiceCollection services, IIdentityServerBuilder builder)
        {
            var apiResources = Configuration.LoadApiResourcesFromSettings();
            builder.AddInMemoryApiResources(apiResources);
        }

        public void AddOperationalStore(IServiceCollection services, IIdentityServerBuilder builder)
        {
            bool useRedis = Convert.ToBoolean(Configuration["appOptions:redis:useRedis"]);
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
                builder.AddInMemoryCaching();
                builder.AddInMemoryPersistedGrants();
                services.AddDistributedMemoryCache();
            }
        }

        public void AddSigningServices(IServiceCollection services, IIdentityServerBuilder builder)
        {
            bool useKeyVault = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVault"]);
            bool useKeyVaultSigning = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVaultSigning"]);
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
        }

        public Action<IdentityServerOptions> GetIdentityServerOptions()
        {
            Action<IdentityServerOptions> identityServerOptions = options =>
            {
                options.InputLengthRestrictions.RefreshToken = 256;
                options.Caching.ClientStoreExpiration = TimeSpan.FromMinutes(5);
            };
            return identityServerOptions;
        }
    }
}
