using System;
using ConferencePlanner.Common.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ConferencePlanner.FrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var hostConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddDockerSecrets(optional: true)
                .AddCommandLine(args)
                .Build();

            var hostBuilder = WebHost.CreateDefaultBuilder(args);
            var instrumentationKey = hostConfig["ApplicationInsights:InstrumentationKey"];
            if (!string.IsNullOrEmpty(instrumentationKey))
            {
                Console.WriteLine("Using Application Insights");
                hostBuilder.UseApplicationInsights(instrumentationKey.Trim());
            }
            else
            {
                hostBuilder.UseApplicationInsights();
            }

            return hostBuilder.UseUrls("http://localhost:5100")
                .UseConfiguration(hostConfig)
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddDockerSecrets(optional: true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    LoggingHelper.RegisterLogging("FrontEnd", logging, context.Configuration);
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
