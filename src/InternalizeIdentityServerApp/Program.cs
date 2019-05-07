using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InternalizeIdentityServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    LoadConfigurations(config, environmentName);
                })
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                });
        public static void LoadConfigurations(IConfigurationBuilder config, string environmentName)
        {
            config
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.redis.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.keyVault.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.IdentityResources.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.ApiResources.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.Clients.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddUserSecrets<Startup>();

        }
    }
}
