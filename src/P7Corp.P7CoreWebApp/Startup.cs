using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModelExtras;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using P7Core;
using P7Core.Extensions;
using P7Corp.P7CoreWebApp.Services;

namespace P7Corp.P7CoreWebApp
{
    public class DefaultHttpClientFactory : IDefaultHttpClientFactory
    {
        private HttpClient _httpClient;
        public HttpMessageHandler HttpMessageHandler { get; set; }

        public HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = new HttpClient()); }
            set { _httpClient = value; }
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.TryAddTransient<IDefaultHttpClientFactory, DefaultHttpClientFactory>();

            services.AddLazyTransient<ISomeLazyTransient, SomeTransient>();
            services.AddLazySingleton<ISomeLazySingleton, SomeSingleton>();
            services.AddLazyScoped<ISomeLazyScoped, SomeScoped>();

            services.AddTransient<ISomeTransient, SomeTransient>();
            services.AddSingleton<ISomeSingleton, SomeSingleton>();
            services.AddScoped<ISomeScoped, SomeScoped>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
          
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

            app.UseMvc();
        }
    }
}
