using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FrontEnd
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
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:55994")
                .UseConfiguration(hostConfig)
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddDockerSecrets();
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
